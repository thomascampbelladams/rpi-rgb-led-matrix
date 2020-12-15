using rpi_rgb_led_matrix_sharp.Helpers;
using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace azure_queue_example
{
    class Program
    {
        private static Scene CreateScene(JsonScene scene, RGBLedMatrix matrix, CanvasHelper screen)
        {
            switch (scene.Type)
            {
                case "horizontal marquee":
                    screen.SetFont($"../../../fonts/{scene.Font}");
                    return new Scene(matrix,
                        scene.AnimationDelay,
                        screen.HorizontalMarqueeText(
                            (string)scene.Content,
                            new Color(scene.Color)));

                case "vertical marquee":
                    screen.SetFont($"../../../fonts/{scene.Font}");
                    return new Scene(matrix,
                            scene.AnimationDelay,
                            screen.VerticalMarqueeText(
                                (string)scene.Content,
                                new Color(scene.Color)));

                case "animation":
                    return new Scene(matrix,
                        scene.AnimationDelay,
                        screen.DoAnimation(
                            ((JArray)scene.Content).ToObject<uint[][][]>(),
                            scene.NumberOfTimesToRepeat,
                            scene.isTwoBitAnimation,
                            (matrix1) =>
                            {
                                if (scene.SceneToShowInBackground != null && !string.IsNullOrEmpty(scene.SceneToShowInBackground.Type))
                                {
                                    return CreateScene(
                                        scene.SceneToShowInBackground,
                                        matrix,
                                        screen).Frames.First();
                                }
                                else
                                {
                                    return screen.CreateNewCanvas();
                                }
                            }));

                case "rainbow transition":
                    return new Scene(matrix, scene.AnimationDelay, screen.RainbowTransition(scene.BlockHeight, scene.BlockWidth));
                    break;

                case "text":
                    screen.SetFont($"../../../fonts/{scene.Font}");
                    if (scene.CenteredVertically && scene.CenteredHorizontally)
                    {
                        return new Scene(
                            matrix,
                            scene.NumberOfSecondsToShow,
                            new List<RGBLedCanvas> {
                                screen.DrawHorizontallyCenteredText(
                                    (string)scene.Content,
                                    new Color(scene.Color))});
                    }
                    else if (scene.CenteredVertically && !scene.CenteredHorizontally)
                    {
                        return new Scene(
                            matrix,
                            scene.NumberOfSecondsToShow,
                            new List<RGBLedCanvas> {
                                screen.DrawVerticallyCenteredText(
                                    (string)scene.Content,
                                    new Color(scene.Color))});
                    }
                    else
                    {

                        return new Scene(
                            matrix,
                            scene.NumberOfSecondsToShow,
                            new List<RGBLedCanvas> {
                                screen.DrawHorizontallyCenteredText(
                                    (string)scene.Content,
                                    new Color(scene.Color))});
                    }

                default:
                    return null;
            }
        }

        private static void WriteNewLine(string message)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("                                                                           ");
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine(message);
        }

        public static int Main(string[] args)
        {
            List<Scene> scenes = new List<Scene>();
            RGBLedMatrix matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = 32,
                Cols = 64,
                HardwareMapping = "adafruit-hat"
            });
            CanvasHelper screen = new CanvasHelper(matrix, 32, 64, "../../../fonts/4x6.bdf");
            string url = @"https://solong.gay/home/DequeueMessage?key=xxxx";
            string deleteUrl = @"https://solong.gay/home/DeleteMessage?key=xxxx&messageId={0}&popReceipt={1}";

            while (true)
            {
                try
                {
                    WriteNewLine("Getting Message");
                    string message = string.Empty;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        message = reader.ReadToEnd();
                        response.Close();
                    }

                    if (message != "{}")
                    {
                        message = message.Substring(1, message.Length - 2).Replace("\\u0022", "\"").Replace("\\u002B", "+").Replace("\\\\", "\\");

                        AzureQueueResponse qResp = JsonConvert.DeserializeObject<AzureQueueResponse>(message);

                        if (!string.IsNullOrEmpty(qResp.MessageText))
                        {
                            WriteNewLine($"Got message id: {qResp.MessageId}. Deserializing...");
                            SceneList sceneList = JsonConvert.DeserializeObject<SceneList>(qResp.MessageText);

                            WriteNewLine($"Got {sceneList.Scenes.Length} scenes back. Creating scenes....");
                            int i = 1;
                            foreach (JsonScene scene in sceneList.Scenes)
                            {
                                WriteNewLine($"Creating {scene.Type}... {((double)i / (double)sceneList.Scenes.Length) * 100}% complete.");

                                scenes.Add(CreateScene(scene, matrix, screen));

                                i++;
                                WriteNewLine($"{scene.Type} created! {((double)i / (double)sceneList.Scenes.Length) * 100}% complete.");
                            }

                            WriteNewLine("Deleting message....");

                            request = (HttpWebRequest)WebRequest.Create(string.Format(deleteUrl, qResp.MessageId, qResp.PopReceipt));
                            request.AutomaticDecompression = DecompressionMethods.GZip;
                            request.Method = "DELETE";
                            request.GetResponse();
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                response.Close();
                            }
                        }
                        else
                        {
                            WriteNewLine($"Got empty message: {message}");
                        }
                    }
                    else
                    {
                        WriteNewLine($"Got empty message: {message}");
                    }

                    WriteNewLine("Rendering Scenes");
                    foreach (Scene scene1 in scenes)
                    {
                        scene1.Render();
                    }
                }
                catch (WebException ex)
                {
                    WriteNewLine("Get Timeout! Retrying in three seconds...");

                    Thread.Sleep(3000);
                }
            }
        }
    }
}

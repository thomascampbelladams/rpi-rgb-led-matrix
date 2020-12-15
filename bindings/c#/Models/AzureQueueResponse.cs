using System;

namespace rpi_rgb_led_matrix_sharp.Models
{
    public class AzureQueueResponse
    {
        public string MessageId { get; set; }
        public string MessageText { get; set; }
        public string PopReceipt { get; set; }
    }
}

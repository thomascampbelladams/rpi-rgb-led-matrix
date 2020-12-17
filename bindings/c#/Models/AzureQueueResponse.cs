using System;

namespace rpi_rgb_led_matrix_sharp.Models
{
    /// <summary>
    /// Queue response from Azure
    /// </summary>
    public class AzureQueueResponse
    {
        /// <summary>
        /// Message ID from the queue
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// Content of the Message
        /// </summary>
        public string MessageText { get; set; }
        /// <summary>
        /// Pop receipt
        /// </summary>
        public string PopReceipt { get; set; }
    }
}

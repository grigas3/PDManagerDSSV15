
namespace PDManager.Common.Models
{
    /// <summary>
    /// Message Type enum
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// SMS Notification
        /// </summary>
        SMS,
        /// <summary>
        /// EMAIL Notification
        /// </summary>
        EMAIL,
        /// <summary>
        ///  Google Cloud Notification
        /// </summary>
        GCM,
        /// <summary>
        /// Firebase Notification
        /// </summary>
        FCM

    }
}

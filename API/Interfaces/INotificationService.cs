using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces;

    public interface INotificationService
    {
    Task<Notification> CreateNotificationAsync(
        string userId,
        string templateKey);

    Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        string userId);

    Task MarkAsReadAsync(
        int notificationId,
        string userId);

    Task MarkAllAsReadAsync(
        string userId);    }

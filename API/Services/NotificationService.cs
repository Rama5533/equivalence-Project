using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class NotificationService:INotificationService
    {
            private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateNotificationAsync(
        string userId,
        string templateKey)
    {
        var template = await _context.NotificationTemplates
            .FirstOrDefaultAsync(t =>
                t.Key == templateKey &&
                t.IsActive);

        if (template == null)
        {
            throw new Exception(
                $"Notification template '{templateKey}' was not found.");
        }

        var notification = new Notification
        {
            UserId = userId,
            TemplateKey = template.Key,
            Title = template.Title,
            Message = template.Message,
            Type = template.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(
        int notificationId,
        string userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == notificationId &&
                n.UserId == userId);

        if (notification == null)
        {
            throw new KeyNotFoundException("Notification not found.");
        }

        notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var notifications = await _context.Notifications
            .Where(n =>
                n.UserId == userId &&
                !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }
    }
}
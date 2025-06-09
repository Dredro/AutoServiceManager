namespace Application.Workers.DTOs;

public record GetWorkerDto
(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    decimal? Salary,
    List<string> ServicesInProgress
);
///TODO add 
//  public ICollection<Notification> Notifications { get; set; } = [];
//public ICollection<Notification> SentNotifications { get; set; } = [];
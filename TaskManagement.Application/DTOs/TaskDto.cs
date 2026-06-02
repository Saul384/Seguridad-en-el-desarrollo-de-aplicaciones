namespace TaskManagement.Application.DTOs;

// Usamos 'record' para crear objetos inmutables ideales para transferencia de datos
public record TaskDto(Guid Id, string Title, string Description, bool IsCompleted, DateTime CreatedAt);
public record CreateTaskRequest(string Title, string Description);
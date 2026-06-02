namespace Project.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // CORRECCIÓN: Inicializamos los strings para que el compilador no marque advertencias de nulos.
    // Entity Framework Core usará este constructor al leer de la base de datos.
    private TaskItem() 
    { 
        Title = string.Empty;
        Description = string.Empty;
    } 

    public TaskItem(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
        
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted() => IsCompleted = true;
}
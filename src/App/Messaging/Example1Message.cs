namespace App.Messaging;

public class Example1Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "example 1";
}

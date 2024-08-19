namespace App.Messaging;

public class Example2Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "example 2";
}

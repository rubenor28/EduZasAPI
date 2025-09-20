public record FieldError
{
    private readonly string _field;
    private readonly string _message;

    public string Field
    {
        get => _field;
        init => _field = value ?? throw new ArgumentNullException(nameof(Field));
    }

    public string Message
    {
        get => _message;
        init => _message = value ?? throw new ArgumentNullException(nameof(Message));
    }

    // Validación adicional en constructor
    public FieldError()
    {
        if (string.IsNullOrWhiteSpace(_field))
            throw new ArgumentException("Name cannot be empty", nameof(Field));

        if (string.IsNullOrWhiteSpace(_message))
            throw new ArgumentException("Message cannot be empty", nameof(Message));
    }
}

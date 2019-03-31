namespace ConsoleApp1
{
    /// <summary>
    ///     Represents the progress of an operation.
    /// </summary>
    internal sealed class Progress
    {
        public int Total { get; set; }

        public int Value { get; set; }

        public override string ToString()
        {
            return $"{nameof(Total)}: {Total}, {nameof(Value)}: {Value}";
        }
    }
}
using DotnetBackendSkeleton.Models.Enums;

namespace DotnetBackendSkeleton.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Status status { get; set; } = Status.TODO;
    }
}

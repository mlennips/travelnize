using LIT.Travelnize.Domain.Trips;

namespace LIT.Travelnize.Domain.Common
{
    public record EntityReference : ValueObject
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = default!;

        public bool IsDestination => Type == nameof(Destination);
        public bool IsAccommodation => Type == nameof(Accommodation);
        public bool IsActivity => Type == nameof(Activity);

        private EntityReference() { }

        public EntityReference(string type, Guid id)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Type cannot be null or whitespace.", nameof(type));
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be an empty GUID.", nameof(id));

            Type = type;
            Id = id;
        }

        public static EntityReference Create<T>(Guid id) where T : IEntity => new(typeof(T).Name, id);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Id;
        }

        public static Type DetectEntityType(string typeName)
        {
            // Optimiert: Suche nur im Assembly von IEntity nach einfachem Namen
            var type = typeof(IEntity).Assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
            if (type != null)
                return type;
            throw new InvalidOperationException($"Type '{typeName}' not found.");
        }
    }
}

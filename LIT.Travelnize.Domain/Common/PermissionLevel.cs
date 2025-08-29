using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record PermissionLevel : SingleValueObject<string>
    {
        public PermissionLevel(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value) || !IsValidPermissionLevel(value))
            {
                throw new ArgumentException("Invalid permission level.");
            }
        }
        private static bool IsValidPermissionLevel(string level)
        {
            return AllLevels.Contains(level);
        }

        public static PermissionLevel Guest { get; } = new PermissionLevel("Guest");
        public static PermissionLevel User { get; } = new PermissionLevel("User");
        public static PermissionLevel Organisator { get; } = new PermissionLevel("Organisator");

        public static IEnumerable<string> AllLevels => ["Guest", "User", "Organisator"];
        public static implicit operator string(PermissionLevel level) => level.Value;
        public static implicit operator PermissionLevel(string value) => new(value);
    }
}
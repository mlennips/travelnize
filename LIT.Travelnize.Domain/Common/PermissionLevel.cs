using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record PermissionLevel : SingleValueObject<string>
    {
        private PermissionLevel(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value) || !IsValidPermissionLevel(value))
            {
                throw new ArgumentException("Invalid permission level.");
            }
        }
        private static bool IsValidPermissionLevel(string level)
        {
            return level == Guest.Value || level == Admin.Value;
        }

        public static PermissionLevel Guest { get; } = new PermissionLevel("Guest");
        public static PermissionLevel Admin { get; } = new PermissionLevel("Admin");

        public static IEnumerable<PermissionLevel> AllLevels => [Guest, Admin];
        public static implicit operator string(PermissionLevel level) => level.Value;
        public static implicit operator PermissionLevel(string value) => new(value);
    }
}
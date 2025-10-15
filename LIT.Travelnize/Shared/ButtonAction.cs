using Microsoft.AspNetCore.Components;

namespace LIT.Travelnize.Shared
{
    public class ButtonAction
    {
        public string Text { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public Action OnClick { get; set; } = default!;
    }
}

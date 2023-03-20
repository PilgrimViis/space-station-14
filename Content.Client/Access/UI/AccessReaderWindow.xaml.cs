using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access;
using Content.Shared.Access.Systems;
using Content.Shared.Roles;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Access.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class AccessReaderWindow : DefaultWindow
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        private readonly AccessReaderBoundUserInterface _owner;

        private readonly Dictionary<string, Button> _accessButtons = new();
        private readonly Dictionary<string, Button> _denyButtons = new();

        public AccessReaderWindow(AccessReaderBoundUserInterface owner, IPrototypeManager prototypeManager,
            List<string> accessLevels, List<string> denyTags)
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            _owner = owner;

            foreach (var access in accessLevels)
            {
                if (!prototypeManager.TryIndex<AccessLevelPrototype>(access, out var accessLevel))
                {
                    Logger.Error($"Unable to find accesslevel for {access}");
                    continue;
                }

                var newButton = new Button
                {
                    Text = GetAccessLevelName(accessLevel),
                    ToggleMode = true,
                };
                AccessLevelGrid.AddChild(newButton);
                _accessButtons.Add(accessLevel.ID, newButton);
                newButton.OnPressed += _ => SubmitData();
            }
            foreach (var access in accessLevels)
            {
                if (!prototypeManager.TryIndex<AccessLevelPrototype>(access, out var accessLevel))
                {
                    Logger.Error($"Unable to find accesslevel for {access}");
                    continue;
                }

                var newButton = new Button
                {
                    Text = GetAccessLevelName(accessLevel),
                    ToggleMode = true,
                };
                DenyTagsGrid.AddChild(newButton);
                _denyButtons.Add(accessLevel.ID, newButton);
                newButton.OnPressed += _ => SubmitData();
            }
        }

        private static string GetAccessLevelName(AccessLevelPrototype prototype)
        {
            if (prototype.Name is { } name)
                return Loc.GetString(name);

            return prototype.ID;
        }

        private void ClearAllAccess()
        {
            foreach (var button in _accessButtons.Values)
            {
                if (button.Pressed)
                {
                    button.Pressed = false;
                }
            }
            foreach (var button in _denyButtons.Values)
            {
                if (button.Pressed)
                {
                    button.Pressed = false;
                }
            }
        }

        public void UpdateState(AccessReaderBoundUserInterfaceState state)
        {
            foreach (var (accessName, button) in _accessButtons)
            {
                button.Disabled = false;
                button.Pressed = state.AccessList?.Contains(accessName) ?? false;
            }
            foreach (var (accessName, button) in _denyButtons)
            {
                button.Disabled = false;
                button.Pressed = state.DenyTags?.Contains(accessName) ?? false;
            }
        }

        private void SubmitData()
        {
        	// Iterate over the buttons dictionary, filter by `Pressed`, only get key from the key/value pair
            _owner.SubmitData(_accessButtons.Where(x => x.Value.Pressed).Select(x => x.Key).ToList(), _denyButtons.Where(x => x.Value.Pressed).Select(x => x.Key).ToList());
        }
    }
}

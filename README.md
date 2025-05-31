# RPG Text Adventure - WinForms Edition

A classic text-based RPG adventure game built with Windows Forms and .NET 9.0. Embark on an epic journey through mystical lands, battle fearsome creatures, and uncover ancient treasures in this immersive text-driven experience.

## 🎮 Features

### Core Gameplay
- **Rich Text-Based Adventure**: Navigate through detailed story scenarios with branching narratives
- **Character Creation & Progression**: Create your hero and develop their skills throughout the journey
- **Combat System**: Engage in turn-based battles with various enemies
- **Inventory Management**: Collect, use, and manage weapons, armor, and magical items
- **Quest System**: Complete main story quests and optional side missions
- **Save/Load System**: Continue your adventure across multiple sessions

### Game Mechanics
- **Multiple Character Classes**: Choose from Warrior, Mage, Rogue, or Cleric
- **Skill Trees**: Unlock new abilities as you level up
- **Equipment System**: Find and equip better gear to enhance your character
- **Magic System**: Cast spells and use magical items
- **Random Events**: Encounter unexpected situations that affect your journey
- **Multiple Endings**: Your choices determine the fate of the realm

## 🖥️ Technical Features

- **Modern WinForms UI**: Clean, intuitive interface optimized for desktop gaming
- **Real-time Text Display**: Smooth text rendering with typing effects
- **Customizable Settings**: Adjust text speed, font size, and color themes
- **Keyboard Shortcuts**: Quick commands for experienced players
- **Auto-save Functionality**: Never lose your progress
- **Modular Design**: Easy to extend with new content and features

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- .NET 9.0 Runtime or later
- Visual Studio 2022 (for development)

### Installation

#### For Players
1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file to your desired location
3. Run `WinFormsApp1.exe` to start your adventure

#### For Developers
1. Clone this repository:
   ```bash
   git clone https://github.com/yourusername/rpg-text-adventure-winforms.git
   cd rpg-text-adventure-winforms
   ```

2. Open the solution in Visual Studio:
   ```bash
   start WinFormsApp1.sln
   ```

3. Build and run the project (F5)

## 🎯 How to Play

### Starting Your Adventure
1. Launch the game and click "New Game"
2. Create your character by choosing:
   - Name
   - Class (Warrior, Mage, Rogue, Cleric)
   - Starting attributes
3. Begin your journey in the village of Elderbrook

### Game Controls
- **Enter**: Confirm actions and advance dialogue
- **Arrow Keys**: Navigate menus
- **Tab**: Quick inventory access
- **Ctrl+S**: Quick save
- **Ctrl+L**: Quick load
- **F1**: Help menu

### Basic Commands
Type commands in the text input area:
- `look` - Examine your surroundings
- `inventory` - Check your items
- `stats` - View character statistics
- `help` - Display available commands
- `save [name]` - Save your game
- `load [name]` - Load a saved game

## 🏗️ Project Structure

```
WinFormsApp1/
├── Forms/
│   ├── Form1.cs              # Main game window
│   ├── CharacterCreation.cs  # Character creation form
│   ├── InventoryForm.cs      # Inventory management
│   └── SettingsForm.cs       # Game settings
├── Game/
│   ├── Engine/
│   │   ├── GameEngine.cs     # Core game logic
│   │   ├── CommandParser.cs  # Text command processing
│   │   └── SaveManager.cs    # Save/load functionality
│   ├── Models/
│   │   ├── Character.cs      # Player character
│   │   ├── Item.cs          # Game items
│   │   ├── Enemy.cs         # Enemy creatures
│   │   └── Location.cs      # Game locations
│   └── Data/
│       ├── Locations.json   # Location definitions
│       ├── Items.json       # Item database
│       └── Enemies.json     # Enemy database
├── Resources/
│   ├── Images/              # Game artwork
│   ├── Sounds/              # Audio files
│   └── Fonts/               # Custom fonts
└── README.md
```

## 🛠️ Development

### Building from Source
1. Ensure you have .NET 9.0 SDK installed
2. Clone the repository
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

### Adding New Content
- **Locations**: Edit `Data/Locations.json` to add new areas
- **Items**: Modify `Data/Items.json` for new equipment and consumables
- **Enemies**: Update `Data/Enemies.json` to introduce new creatures
- **Quests**: Implement new quest logic in the `Game/Engine/QuestManager.cs`

### Code Style
- Follow C# naming conventions
- Use XML documentation for public methods
- Implement proper error handling
- Write unit tests for game logic

## 🎨 Customization

### Themes
The game supports multiple visual themes:
- **Classic**: Traditional green-on-black terminal style
- **Modern**: Clean white background with dark text
- **Fantasy**: Mystical purple and gold color scheme
- **Custom**: Create your own color combinations

### Mods
The game is designed to be moddable:
- JSON-based content files for easy editing
- Plugin system for custom game mechanics
- Scripting support for complex interactions

## 🐛 Troubleshooting

### Common Issues
- **Game won't start**: Ensure .NET 9.0 runtime is installed
- **Save files not loading**: Check file permissions in the save directory
- **Performance issues**: Try reducing text animation speed in settings

### Getting Help
- Check the [Wiki](../../wiki) for detailed guides
- Report bugs in the [Issues](../../issues) section
- Join our [Discord community](https://discord.gg/your-server) for support

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Ways to Contribute
- Report bugs and suggest features
- Improve documentation
- Add new game content (locations, items, quests)
- Optimize performance
- Create artwork and sound effects

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Inspired by classic text adventures like Zork and Adventure
- Built with the powerful .NET ecosystem
- Special thanks to the WinForms community for continued support

## 📊 Roadmap

### Version 2.0 (Planned)
- [ ] Multiplayer support
- [ ] Advanced graphics mode
- [ ] Voice narration
- [ ] Mobile companion app
- [ ] Steam Workshop integration

### Version 1.5 (In Development)
- [ ] Enhanced combat system
- [ ] New character classes
- [ ] Expanded world map
- [ ] Achievement system

---

**Ready to begin your adventure?** Download the game and step into a world of endless possibilities!

*"In the realm of Aethermoor, legends are not born—they are forged through courage, wisdom, and the choices you make."* 
using System;

namespace WinFormsApp1.Constants
{
    public static class GameConstants
    {
        // Game Information
        public const string GAME_TITLE = "Realm of Aethermoor";
        public const string GAME_VERSION = "1.0";
        
        // Default Locations
        public const string DEFAULT_LOCATION = "village";
        public const string VILLAGE_LOCATION = "village";
        public const string FOREST_LOCATION = "forest";
        public const string PLAINS_LOCATION = "plains";
        public const string CAVE_LOCATION = "cave";
        public const string RUINS_LOCATION = "ruins";
        public const string LAIR_LOCATION = "lair";
        
        // Character Classes
        public const string WARRIOR_CLASS = "warrior";
        public const string MAGE_CLASS = "mage";
        public const string ROGUE_CLASS = "rogue";
        public const string CLERIC_CLASS = "cleric";
        
        // Item Types
        public const string WEAPON_TYPE = "weapon";
        public const string ARMOR_TYPE = "armor";
        public const string POTION_TYPE = "potion";
        public const string KEY_TYPE = "key";
        public const string QUEST_TYPE = "quest";
        public const string MISC_TYPE = "misc";
        
        // Common Item Names
        public const string HEALTH_POTION = "Health Potion";
        public const string IRON_SWORD = "Iron Sword";
        public const string IRON_SHIELD = "Iron Shield";
        public const string HUNTING_BOW = "Hunting Bow";
        public const string STEEL_DAGGER = "Steel Dagger";
        public const string LEATHER_ARMOR = "Leather Armor";
        public const string MAGIC_STAFF = "Magic Staff";
        public const string MANA_POTION = "Mana Potion";
        public const string LOCKPICKS = "Lockpicks";
        public const string HOLY_MACE = "Holy Mace";
        public const string HEALING_POTION = "Healing Potion";
        
        // Enemy Names
        public const string GOBLIN = "goblin";
        public const string ORC = "orc";
        public const string TROLL = "troll";
        public const string DRAGON = "dragon";
        public const string WOLF = "wolf";
        public const string BANDIT = "bandit";
        
        // Cheat Activation Commands
        public const string CHEAT_IDDQD = "iddqd";
        public const string CHEAT_IDKFA = "idkfa";
        public const string CHEAT_THEREISNOSPOON = "thereisnospoon";
        public const string CHEAT_KONAMI = "konami";
        public const string CHEAT_UP = "up";
        public const string CHEAT_DOWN = "down";
        public const string CHEAT_LEFT = "left";
        public const string CHEAT_RIGHT = "right";
        public const string CHEAT_B = "b";
        public const string CHEAT_A = "a";
        
        // Game Commands
        public const string CMD_LOOK = "look";
        public const string CMD_LOOK_SHORT = "l";
        public const string CMD_GO = "go";
        public const string CMD_MOVE = "move";
        public const string CMD_NORTH = "north";
        public const string CMD_SOUTH = "south";
        public const string CMD_EAST = "east";
        public const string CMD_WEST = "west";
        public const string CMD_INVENTORY = "inventory";
        public const string CMD_INV = "inv";
        public const string CMD_I = "i";
        public const string CMD_STATS = "stats";
        public const string CMD_STATUS = "status";
        public const string CMD_TAKE = "take";
        public const string CMD_GET = "get";
        public const string CMD_USE = "use";
        public const string CMD_ATTACK = "attack";
        public const string CMD_FIGHT = "fight";
        public const string CMD_HELP = "help";
        public const string CMD_CHEAT = "cheat";
        public const string CMD_CHEATS = "cheats";
        public const string CMD_SAVE = "save";
        public const string CMD_LOAD = "load";
        public const string CMD_SAVES = "saves";
        public const string CMD_LIST = "list";
        public const string CMD_SKILLS = "skills";
        public const string CMD_SKILL = "skill";
        public const string CMD_QUIT = "quit";
        public const string CMD_EXIT = "exit";
        public const string CMD_DEFEND = "defend";
        public const string CMD_BLOCK = "block";
        public const string CMD_FLEE = "flee";
        public const string CMD_RUN = "run";
        
        // Cheat Commands
        public const string CHEAT_CMD_HELP = "cheathelp";
        public const string CHEAT_CMD_GODMODE = "godmode";
        public const string CHEAT_CMD_GOD = "god";
        public const string CHEAT_CMD_INFINITE_HEALTH = "infinitehealth";
        public const string CHEAT_CMD_INF_HEALTH = "infhealth";
        public const string CHEAT_CMD_INFINITE_GOLD = "infinitegold";
        public const string CHEAT_CMD_INF_GOLD = "infgold";
        public const string CHEAT_CMD_ADD_GOLD = "addgold";
        public const string CHEAT_CMD_GOLD = "gold";
        public const string CHEAT_CMD_ADD_EXP = "addexp";
        public const string CHEAT_CMD_EXP = "exp";
        public const string CHEAT_CMD_EXPERIENCE = "experience";
        public const string CHEAT_CMD_LEVEL_UP = "levelup";
        public const string CHEAT_CMD_LVL_UP = "lvlup";
        public const string CHEAT_CMD_SET_LEVEL = "setlevel";
        public const string CHEAT_CMD_LEVEL = "level";
        public const string CHEAT_CMD_HEAL = "heal";
        public const string CHEAT_CMD_FULL_HEAL = "fullheal";
        public const string CHEAT_CMD_ADD_ITEM = "additem";
        public const string CHEAT_CMD_GIVE_ITEM = "giveitem";
        public const string CHEAT_CMD_ITEM = "item";
        public const string CHEAT_CMD_CLEAR_INVENTORY = "clearinventory";
        public const string CHEAT_CMD_CLEAR_INV = "clearinv";
        public const string CHEAT_CMD_TELEPORT = "teleport";
        public const string CHEAT_CMD_TP = "tp";
        public const string CHEAT_CMD_GOTO = "goto";
        public const string CHEAT_CMD_NOCLIP = "noclip";
        public const string CHEAT_CMD_SPAWN_ENEMY = "spawnenemy";
        public const string CHEAT_CMD_ADD_ENEMY = "addenemy";
        public const string CHEAT_CMD_KILL_ALL_ENEMIES = "killallenemies";
        public const string CHEAT_CMD_KILL_ENEMIES = "killenemies";
        public const string CHEAT_CMD_MAX_STATS = "maxstats";
        public const string CHEAT_CMD_SET_STATS = "setstats";
        public const string CHEAT_CMD_SHOW_DEBUG = "showdebug";
        public const string CHEAT_CMD_DEBUG = "debug";
        public const string CHEAT_CMD_RESET_GAME = "resetgame";
        public const string CHEAT_CMD_DISABLE_CHEATS = "disablecheats";
        
        // Save File Names
        public const string QUICK_SAVE = "QuickSave";
        public const string AUTO_SAVE_PREFIX = "AutoSave_";
        public const string QUICK_SAVE_COMMAND = "quicksave";
        
        // File Paths and Extensions
        public const string DATA_PATH = "Assets/Data";
        public const string ITEMS_JSON = "Items.json";
        public const string ENEMIES_JSON = "Enemies.json";
        public const string LOCATIONS_JSON = "Locations.json";
        public const string CHARACTER_CLASSES_JSON = "CharacterClasses.json";
        public const string JSON_EXTENSION = ".json";
        
        // Save Directory Structure
        public const string GAME_FOLDER_NAME = "RealmOfAethermoor";
        public const string SAVE_FOLDER_NAME = "SavedGames";
        
        // Default Values
        public const int DEFAULT_GOLD_AMOUNT = 1000;
        public const int DEFAULT_EXP_AMOUNT = 100;
        public const int DEFAULT_LEVEL_UP_COUNT = 1;
        public const int MAX_LEVEL = 100;
        public const int MIN_LEVEL = 1;
        public const int MAX_STAT_VALUE = 9999;
        public const int MIN_STAT_VALUE = 1;
        public const int MAX_GOLD = 999999;
        public const int MAX_SKILL_POINTS = 999;
        public const double RANDOM_ENCOUNTER_CHANCE = 0.3;
        public const double FLEE_SUCCESS_RATE = 0.7;
        
        // Combat Messages
        public const string COMBAT_COMMANDS_MSG = "Combat commands: attack, defend, use [item], flee";
        public const string COMBAT_BEGIN_MSG = "Combat begins!";
        public const string COMBAT_VICTORY_MSG = "You defeated the";
        public const string COMBAT_DEFEAT_MSG = "You have been defeated!";
        public const string COMBAT_FLEE_SUCCESS_MSG = "You successfully flee from combat!";
        public const string COMBAT_FLEE_FAIL_MSG = "You failed to escape!";
        
        // UI Messages
        public const string WELCOME_MSG = "=== Welcome to the Realm of Aethermoor ===";
        public const string HELP_MSG = "=== Available Commands ===";
        public const string CHEAT_ACTIVATED_MSG = "🎮 CHEAT CODES ACTIVATED! 🎮";
        public const string CHEAT_HELP_MSG = "=== CHEAT COMMANDS ===";
        public const string CHEAT_ACTIVATION_MSG = "=== CHEAT CODE ACTIVATION ===";
        public const string DEBUG_INFO_MSG = "=== DEBUG INFORMATION ===";
        public const string INVENTORY_MSG = "=== Inventory ===";
        public const string CHARACTER_STATS_MSG = "=== Character Stats ===";
        public const string LEVEL_UP_MSG = "🎉 LEVEL UP! You are now level";
        
        // Error Messages
        public const string UNKNOWN_COMMAND_MSG = "I don't understand that command. Type 'help' for available commands.";
        public const string NO_SUCH_ITEM_MSG = "There's no such item here.";
        public const string NO_SUCH_ENEMY_MSG = "There's no such enemy here.";
        public const string CANT_GO_THAT_WAY_MSG = "You can't go that way.";
        public const string NOTHING_TO_ATTACK_MSG = "There's nothing to attack here.";
        public const string INVENTORY_EMPTY_MSG = "Your inventory is empty.";
        public const string NOT_ENOUGH_GOLD_MSG = "Not enough gold!";
        public const string START_GAME_FIRST_MSG = "Start a new game first!";
        public const string START_GAME_FOR_CHEATS_MSG = "Start a game first to use cheats!";
        
        // Date/Time Formats
        public const string SAVE_TIMESTAMP_FORMAT = "yyyy-MM-dd_HH-mm-ss";
        public const string DISPLAY_TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string FILE_TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm";
    }
    
    public static class CheatConstants
    {
        // Konami Code Sequence
        public static readonly string[] KONAMI_CODE_SEQUENCE = 
        {
            GameConstants.CHEAT_KONAMI,
            GameConstants.CHEAT_UP,
            GameConstants.CHEAT_UP,
            GameConstants.CHEAT_DOWN,
            GameConstants.CHEAT_DOWN,
            GameConstants.CHEAT_LEFT,
            GameConstants.CHEAT_RIGHT,
            GameConstants.CHEAT_LEFT,
            GameConstants.CHEAT_RIGHT,
            GameConstants.CHEAT_B,
            GameConstants.CHEAT_A
        };
        
        // Classic Cheat Codes
        public static readonly string[] CLASSIC_CHEAT_CODES = 
        {
            GameConstants.CHEAT_IDDQD,
            GameConstants.CHEAT_IDKFA,
            GameConstants.CHEAT_THEREISNOSPOON
        };
    }
} 
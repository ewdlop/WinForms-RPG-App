# Realm of Aethermoor | 艾瑟摩尔王国
### A Classic Text-Based RPG Adventure | 经典文字冒险RPG游戏

**Version 3.0** | **版本 3.0**

![WinForms](https://img.shields.io/badge/WinForms-.NET%209.0-blue) ![C#](https://img.shields.io/badge/C%23-Latest-green) ![License](https://img.shields.io/badge/License-MIT-yellow) ![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-purple) ![DI](https://img.shields.io/badge/DI-.NET%20Generic%20Host-orange)

---

## 📖 Overview | 概述

Realm of Aethermoor is a modern take on classic text-based RPG adventures, built with .NET WinForms and featuring a sophisticated event-driven architecture with dependency injection. Experience a rich fantasy world where your choices shape your destiny through immersive storytelling, strategic combat, and character progression, all powered by a clean, maintainable codebase.

艾瑟摩尔王国是一款基于经典文字冒险RPG的现代化游戏，使用 .NET WinForms 构建，具有复杂的事件驱动架构和依赖注入。通过沉浸式的故事叙述、策略性战斗和角色成长，体验一个丰富的奇幻世界，所有这些都由干净、可维护的代码库提供支持。

---

## ✨ Features | 特色功能

### 🏗️ **Advanced Architecture** | **高级架构** (NEW! | 新功能!)
- **Event-Driven Design** | **事件驱动设计**: Complete separation of concerns with 25+ event types | 通过25+事件类型完全分离关注点
- **.NET Generic Host** | **.NET通用主机**: Microsoft's dependency injection container | 微软的依赖注入容器
- **Manager Pattern** | **管理器模式**: 6 specialized managers for different game aspects | 6个专门的管理器处理不同游戏方面
- **GameCoordinatorService** | **游戏协调服务**: Advanced service for complex multi-manager operations | 复杂多管理器操作的高级服务
- **Decorator Pattern** | **装饰器模式**: Logging and validation decorators for managers | 管理器的日志和验证装饰器

### 🎭 **Character Creation & Classes** | **角色创建与职业**
- **4 Unique Classes** | **4个独特职业**: Warrior (战士), Mage (法师), Rogue (盗贼), Cleric (牧师)
- **Customizable Names** | **自定义姓名**: Create your unique hero | 创造您独特的英雄
- **Class-Specific Stats** | **职业专属属性**: Each class has unique starting attributes | 每个职业都有独特的初始属性

### ⚔️ **Combat System** | **战斗系统**
- **Turn-Based Combat** | **回合制战斗**: Strategic battles with attack, defend, and flee options | 具有攻击、防御和逃跑选项的策略性战斗
- **Equipment System** | **装备系统**: Weapons and armor affect your combat effectiveness | 武器和护甲影响您的战斗效果
- **Enemy Variety** | **敌人种类**: Face wolves, boars, goblins, orcs, and dragons | 面对狼、野猪、哥布林、兽人和巨龙

### 🌟 **Skill Tree System** | **技能树系统**
- **4 Skill Categories** | **4个技能类别**: Combat (战斗), Magic (魔法), Defense (防御), Utility (实用)
- **14 Unique Skills** | **14个独特技能**: Learn powerful abilities to enhance your character | 学习强大技能来增强您的角色
- **Skill Points** | **技能点数**: Earn points through leveling and spend them wisely | 通过升级获得点数并明智地使用
- **Prerequisites** | **前置条件**: Advanced skills require mastering basic ones first | 高级技能需要先掌握基础技能

### 🗺️ **World Exploration** | **世界探索**
- **Interactive Map** | **交互式地图**: Visual world map with location details | 带有位置详情的可视化世界地图
- **6 Unique Locations** | **6个独特地点**: From peaceful villages to dangerous dragon lairs | 从和平村庄到危险的龙穴
- **Random Encounters** | **随机遭遇**: Dynamic encounter system with level-based chances | 基于等级几率的动态遭遇系统

### 🎒 **Inventory & Items** | **物品栏与道具**
- **Professional Inventory UI** | **专业物品栏界面**: Color-coded items with detailed descriptions | 带有详细描述的彩色编码物品
- **Item Categories** | **物品类别**: Weapons (武器), Armor (护甲), Potions (药水), Quest Items (任务物品)
- **Equipment Management** | **装备管理**: Easy equip/unequip with stat preview | 轻松装备/卸下并预览属性

### 💾 **Save System** | **存档系统**
- **Multiple Save Slots** | **多个存档槽位**: Create and manage multiple game saves | 创建和管理多个游戏存档
- **Auto-Save Feature** | **自动存档功能**: Timestamped automatic saves | 带时间戳的自动存档
- **Safe Directory** | **安全目录**: Saves stored in Documents/RealmOfAethermoor | 存档保存在文档/RealmOfAethermoor中

### 🎨 **User Interface** | **用户界面**
- **Modern WinForms Design** | **现代WinForms设计**: Professional split-panel layout | 专业的分割面板布局
- **Multiple Themes** | **多种主题**: Classic, Modern, Fantasy, Dark, High Contrast | 经典、现代、奇幻、黑暗、高对比度
- **Keyboard Shortcuts** | **键盘快捷键**: Quick access to common actions | 快速访问常用操作
- **Real-time Stats** | **实时状态**: Live health, experience, and character info | 实时健康、经验和角色信息

### 🛠️ **Asset Editor** | **资源编辑器**
- **TreeView-Based JSON Editor** | **基于TreeView的JSON编辑器**: Visual editing of game data | 游戏数据的可视化编辑
- **Real-time Validation** | **实时验证**: Type checking and data integrity | 类型检查和数据完整性
- **Add/Delete Nodes** | **添加/删除节点**: Modify game content easily | 轻松修改游戏内容
- **Professional Interface** | **专业界面**: Color-coded icons and hierarchical display | 彩色编码图标和分层显示

### 🔧 **Advanced Commands** | **高级命令** (NEW! | 新功能!)
- **auto-explore** | **自动探索**: Automatically explore random directions | 自动探索随机方向
- **optimize-character** | **优化角色**: Auto-equip best available items | 自动装备最佳可用物品
- **batch-use <type>** | **批量使用 <类型>**: Use all items of specified type | 使用指定类型的所有物品
- **skill-combo** | **技能连击**: Execute multiple skills in sequence | 按顺序执行多个技能

---

## 🚀 Installation | 安装

### Prerequisites | 前置要求
- **.NET 9.0 Runtime** | **.NET 9.0 运行时**
- **Windows 10/11** | **Windows 10/11**
- **Minimum 4GB RAM** | **最少4GB内存**

### Quick Setup | 快速设置

1. **Clone the repository** | **克隆仓库**:
   ```bash
   git clone https://github.com/yourusername/WinFormsApp1.git
   cd WinFormsApp1
   ```

2. **Build the project** | **构建项目**:
   ```bash
   dotnet build --configuration Release
   ```

3. **Run the game** | **运行游戏**:
   ```bash
   dotnet run
   ```

### Alternative | 备选方案
Download the latest release from the [Releases](https://github.com/yourusername/WinFormsApp1/releases) page and run the executable directly.

从 [发布页面](https://github.com/yourusername/WinFormsApp1/releases) 下载最新版本并直接运行可执行文件。

---

## 🎮 Gameplay Guide | 游戏指南

### Getting Started | 开始游戏

1. **Create Your Character** | **创建角色**
   - Choose from 4 unique classes | 从4个独特职业中选择
   - Enter your hero's name | 输入您英雄的姓名
   - Review starting stats | 查看初始属性

2. **Learn the Commands** | **学习命令**
   ```
   Movement | 移动: north, south, east, west, go [direction]
   Interaction | 交互: look, take [item], use [item]
   Combat | 战斗: attack [enemy], defend, flee
   Character | 角色: inventory, stats, skills
   Game | 游戏: save [name], load [name], help, quit
   Advanced | 高级: auto-explore, optimize-character, batch-use <type>
   ```

3. **Explore the World** | **探索世界**
   - Use `look` to examine your surroundings | 使用 `look` 查看周围环境
   - Pick up items with `take [item]` | 用 `take [物品]` 拾取物品
   - Fight enemies to gain experience | 战斗敌人获得经验

4. **Manage Your Character** | **管理角色**
   - Open inventory with `inventory` or Tab key | 用 `inventory` 或Tab键打开背包
   - Check stats with `stats` command | 用 `stats` 命令查看属性
   - Learn skills with `skills` command | 用 `skills` 命令学习技能

### Advanced Features | 高级功能

#### Advanced Menu System | 高级菜单系统
Access through the **Advanced** menu in the main window | 通过主窗口的**高级**菜单访问
- **Validate Game State** | **验证游戏状态**: Check for data consistency issues | 检查数据一致性问题
- **Comprehensive Status** | **综合状态**: View detailed system information | 查看详细系统信息
- **Synchronize Managers** | **同步管理器**: Ensure all systems are in sync | 确保所有系统同步
- **Perform Maintenance** | **执行维护**: Automated system cleanup and optimization | 自动系统清理和优化

#### GameCoordinatorService Features | 游戏协调服务功能
- **Level Up with Rewards** | **升级奖励**: Automated leveling with bonus gold | 自动升级并获得奖励金币
- **Auto Explore** | **自动探索**: Intelligent exploration with encounter handling | 智能探索并处理遭遇
- **Character Optimization** | **角色优化**: Automatic equipment optimization | 自动装备优化

#### Skill Tree System | 技能树系统
- **Combat Skills** | **战斗技能**: Enhance your fighting prowess | 提升您的战斗能力
- **Magic Skills** | **魔法技能**: Learn powerful spells | 学习强大法术
- **Defense Skills** | **防御技能**: Improve your survivability | 提高您的生存能力
- **Utility Skills** | **实用技能**: Unlock special abilities | 解锁特殊能力

#### Asset Editor | 资源编辑器
Access through `Tools → Asset Editor` | 通过 `工具 → 资源编辑器` 访问
- Edit items, enemies, locations, and character classes | 编辑物品、敌人、地点和角色职业
- Visual TreeView interface for JSON data | JSON数据的可视化TreeView界面
- Real-time validation and error checking | 实时验证和错误检查

---

## 📁 Project Structure | 项目结构

```
WinFormsApp1/
├── src/                          # Source code | 源代码
│   ├── Forms/                    # UI Forms | 界面表单
│   │   ├── Form1.cs             # Main game window | 主游戏窗口
│   │   ├── InventoryForm.cs     # Inventory management | 背包管理
│   │   ├── MapForm.cs           # World map display | 世界地图显示
│   │   ├── SkillTreeForm.cs     # Skill tree interface | 技能树界面
│   │   ├── SettingsForm.cs      # Game settings | 游戏设置
│   │   └── AssetEditorForm.cs   # Asset editor | 资源编辑器
│   ├── Controls/                 # Custom controls | 自定义控件
│   │   ├── CharacterStatsControl.cs    # Character stats display | 角色属性显示
│   │   ├── ProgressDisplayControl.cs   # Progress bars | 进度条
│   │   ├── QuickActionsControl.cs      # Quick action buttons | 快速操作按钮
│   │   ├── SkillTreeControl.cs         # Skill tree TreeView | 技能树TreeView
│   │   └── JsonAssetEditorControl.cs   # JSON editor control | JSON编辑器控件
│   ├── Game/                     # Game logic | 游戏逻辑
│   │   └── GameEngine.cs        # Legacy game engine (refactored) | 传统游戏引擎（已重构）
│   ├── Managers/                 # Business logic managers | 业务逻辑管理器
│   │   ├── BaseManager.cs       # Base manager class | 基础管理器类
│   │   ├── EventManager.cs      # Central event bus | 中央事件总线
│   │   ├── GameManager.cs       # Game state management | 游戏状态管理
│   │   ├── PlayerManager.cs     # Player progression | 玩家进度
│   │   ├── CombatManager.cs     # Combat system | 战斗系统
│   │   ├── InventoryManager.cs  # Inventory operations | 背包操作
│   │   ├── LocationManager.cs   # World navigation | 世界导航
│   │   ├── SkillManager.cs      # Skill tree logic | 技能树逻辑
│   │   └── SaveManager.cs       # Save/Load operations | 存档/读档操作
│   ├── Services/                 # Advanced services | 高级服务
│   │   └── GameCoordinatorService.cs  # Multi-manager coordination | 多管理器协调
│   ├── Extensions/               # Dependency injection extensions | 依赖注入扩展
│   │   └── ServiceCollectionExtensions.cs  # Service registration | 服务注册
│   ├── Interfaces/               # Manager interfaces | 管理器接口
│   │   ├── IBaseManager.cs      # Base manager interface | 基础管理器接口
│   │   ├── IGameManager.cs      # Game manager interface | 游戏管理器接口
│   │   ├── IPlayerManager.cs    # Player manager interface | 玩家管理器接口
│   │   ├── ICombatManager.cs    # Combat manager interface | 战斗管理器接口
│   │   ├── IInventoryManager.cs # Inventory manager interface | 背包管理器接口
│   │   ├── ILocationManager.cs  # Location manager interface | 位置管理器接口
│   │   ├── ISkillManager.cs     # Skill manager interface | 技能管理器接口
│   │   └── IGameCoordinatorService.cs  # Coordinator service interface | 协调服务接口
│   ├── Events/                   # Event definitions | 事件定义
│   │   ├── BaseEvents.cs        # Base event classes | 基础事件类
│   │   ├── GameStateEvents.cs   # Game state events | 游戏状态事件
│   │   ├── PlayerEvents.cs      # Player-related events | 玩家相关事件
│   │   ├── CombatEvents.cs      # Combat events | 战斗事件
│   │   ├── InventoryEvents.cs   # Inventory events | 背包事件
│   │   ├── LocationEvents.cs    # Location events | 位置事件
│   │   └── SkillEvents.cs       # Skill events | 技能事件
│   ├── Models/                   # Data models | 数据模型
│   │   ├── GameModels.cs        # Game entities | 游戏实体
│   │   └── JsonDataModels.cs    # JSON data structures | JSON数据结构
│   ├── Data/                     # Data management | 数据管理
│   │   └── DataLoader.cs        # JSON data loader | JSON数据加载器
│   └── Dialogs/                  # Dialog windows | 对话框窗口
│       ├── CharacterCreationDialog.cs  # Character creation | 角色创建
│       ├── ThemeSelectionDialog.cs     # Theme selection | 主题选择
│       └── AddNodeDialog.cs            # Add JSON node | 添加JSON节点
├── Assets/                       # Game assets | 游戏资源
│   └── Data/                    # JSON data files | JSON数据文件
│       ├── CharacterClasses.json  # Character class definitions | 角色职业定义
│       ├── Items.json            # Item database | 物品数据库
│       ├── Enemies.json          # Enemy definitions | 敌人定义
│       └── Locations.json        # Game world locations | 游戏世界地点
├── Tests/                        # Unit tests | 单元测试
│   ├── Unit/                    # Unit tests | 单元测试
│   ├── Integration/             # Integration tests | 集成测试
│   └── UI/                      # UI tests | UI测试
├── docs/                         # Documentation | 文档
│   ├── README.md                # This file | 本文件
│   ├── REFACTORING_PLAN.md      # Event-driven refactoring plan | 事件驱动重构计划
│   └── REFACTORING_PROGRESS.md  # Progress tracking | 进度跟踪
└── WinFormsApp1.csproj          # Project file | 项目文件
```

---

## 🎯 Development Roadmap | 开发路线图

### ✅ Completed Features | 已完成功能
- [x] Character creation system | 角色创建系统
- [x] Turn-based combat | 回合制战斗
- [x] Inventory management | 背包管理
- [x] Save/Load system | 存档/读档系统
- [x] Skill tree with TreeView | 带TreeView的技能树
- [x] World map with visual interface | 带可视化界面的世界地图
- [x] Equipment system | 装备系统
- [x] Custom controls architecture | 自定义控件架构
- [x] Asset editor with TreeView JSON editing | 带TreeView JSON编辑的资源编辑器
- [x] **Event-driven architecture** | **事件驱动架构**
- [x] **Dependency injection with .NET Generic Host** | **使用.NET通用主机的依赖注入**
- [x] **Manager pattern with 6 specialized managers** | **6个专门管理器的管理器模式**
- [x] **GameCoordinatorService for complex operations** | **复杂操作的游戏协调服务**
- [x] **Advanced command system** | **高级命令系统**

### 🚧 In Progress | 进行中
- [ ] Enhanced combat animations | 增强战斗动画
- [ ] Quest system implementation | 任务系统实现
- [ ] Advanced NPC interactions | 高级NPC交互

### ✅ **Version 3.0 - Event-Driven Architecture** | **版本 3.0 - 事件驱动架构** (COMPLETED! | 已完成!)
**📋 [Detailed Refactoring Plan](docs/REFACTORING_PLAN.md)** | **📋 [详细重构计划](docs/REFACTORING_PLAN.md)**
**📊 [Progress Tracking](docs/REFACTORING_PROGRESS.md)** | **📊 [进度跟踪](docs/REFACTORING_PROGRESS.md)**

#### ✅ Phase 1: Foundation | 第一阶段：基础 (100% Complete | 100%完成)
- [x] Implement EventManager (central event bus) | 实现EventManager（中央事件总线）
- [x] Create base manager classes and interfaces | 创建基础管理器类和接口
- [x] Extract GameManager from GameEngine | 从GameEngine提取GameManager
- [x] Implement PlayerManager for character progression | 实现PlayerManager用于角色进度
- [x] Set up dependency injection container | 设置依赖注入容器

#### ✅ Phase 2: Core Managers | 第二阶段：核心管理器 (100% Complete | 100%完成)
- [x] Implement CombatManager for battle logic | 实现CombatManager用于战斗逻辑
- [x] Create InventoryManager for item operations | 创建InventoryManager用于物品操作
- [x] Build LocationManager for world navigation | 构建LocationManager用于世界导航
- [x] Develop SkillManager for skill tree logic | 开发SkillManager用于技能树逻辑
- [x] Refactor GameEngine to use managers | 重构GameEngine使用管理器

#### ✅ Phase 3: UI Refactoring | 第三阶段：UI重构 (100% Complete | 100%完成)
- [x] Update all forms to use event-driven architecture | 更新所有表单使用事件驱动架构
- [x] Remove direct GameEngine dependencies from UI | 移除UI对GameEngine的直接依赖
- [x] Implement proper data binding with events | 实现事件的适当数据绑定
- [x] Enhance custom controls for event handling | 增强自定义控件的事件处理

#### ✅ Phase 4: Advanced Features | 第四阶段：高级功能 (100% Complete | 100%完成)
- [x] Add GameCoordinatorService for complex operations | 添加复杂操作的游戏协调服务
- [x] Implement .NET Generic Host dependency injection | 实现.NET通用主机依赖注入
- [x] Create decorator pattern for logging and validation | 创建日志和验证的装饰器模式
- [x] Add advanced command system | 添加高级命令系统

### 🔮 **Version 4.0 - Advanced Features** | **版本 4.0 - 高级功能** (PLANNED | 计划中)

#### Phase 1: Enhanced Gameplay | 第一阶段：增强游戏玩法
- [ ] Quest system with branching storylines | 带分支故事线的任务系统
- [ ] Advanced NPC dialogue system | 高级NPC对话系统
- [ ] Dynamic world events | 动态世界事件
- [ ] Achievement system | 成就系统

#### Phase 2: Technical Improvements | 第二阶段：技术改进
- [ ] Plugin system for mods | 模组插件系统
- [ ] Performance monitoring and optimization | 性能监控和优化
- [ ] Automated testing framework | 自动化测试框架
- [ ] Configuration management system | 配置管理系统

### 📋 Planned Features | 计划功能
- [ ] Multiplayer support | 多人游戏支持
- [ ] Voice narration | 语音叙述
- [ ] Data validation tool | 数据验证工具
- [ ] Import/Export game data | 导入/导出游戏数据
- [ ] Mobile companion app | 移动伴侣应用

---

## 🏗️ Architecture Overview | 架构概述

### Event-Driven Design | 事件驱动设计

The game uses a sophisticated event-driven architecture that provides complete separation of concerns and loose coupling between components.

游戏使用复杂的事件驱动架构，提供完全的关注点分离和组件之间的松耦合。

#### Core Components | 核心组件

1. **EventManager** | **事件管理器**
   - Central event bus for all game communications | 所有游戏通信的中央事件总线
   - Type-safe event publishing and subscription | 类型安全的事件发布和订阅
   - Thread-safe operations | 线程安全操作

2. **Manager Pattern** | **管理器模式**
   - **GameManager**: Game state and flow control | 游戏状态和流程控制
   - **PlayerManager**: Character progression and stats | 角色进度和属性
   - **CombatManager**: Battle logic and mechanics | 战斗逻辑和机制
   - **InventoryManager**: Item management and equipment | 物品管理和装备
   - **LocationManager**: World navigation and exploration | 世界导航和探索
   - **SkillManager**: Skill tree and abilities | 技能树和能力

3. **GameCoordinatorService** | **游戏协调服务**
   - Orchestrates complex operations across multiple managers | 协调多个管理器的复杂操作
   - Provides advanced features like auto-exploration and character optimization | 提供自动探索和角色优化等高级功能
   - Implements validation and maintenance operations | 实现验证和维护操作

### Dependency Injection | 依赖注入

The application uses Microsoft's .NET Generic Host for dependency injection, providing:

应用程序使用微软的.NET通用主机进行依赖注入，提供：

- **Service Lifetime Management** | **服务生命周期管理**: Singleton, Transient, and Scoped services | 单例、瞬态和作用域服务
- **Interface-Based Design** | **基于接口的设计**: All managers implement interfaces for testability | 所有管理器实现接口以便测试
- **Decorator Pattern** | **装饰器模式**: Logging and validation decorators | 日志和验证装饰器
- **Clean Architecture** | **清洁架构**: Separation of concerns and dependency inversion | 关注点分离和依赖倒置

### Event Types | 事件类型

The system includes 25+ event types organized into categories:

系统包括25+种事件类型，分为以下类别：

- **Game State Events** | **游戏状态事件**: Game start, pause, end | 游戏开始、暂停、结束
- **Player Events** | **玩家事件**: Level up, stat changes, health updates | 升级、属性变化、健康更新
- **Combat Events** | **战斗事件**: Combat start/end, damage, flee attempts | 战斗开始/结束、伤害、逃跑尝试
- **Inventory Events** | **背包事件**: Item add/remove, equipment changes | 物品添加/移除、装备变化
- **Location Events** | **位置事件**: Movement, exploration, encounters | 移动、探索、遭遇
- **Skill Events** | **技能事件**: Skill learning, usage, point changes | 技能学习、使用、点数变化

---

## 🛠️ Development Guidelines | 开发指南

### Code Style | 代码风格
- **C# Naming Conventions** | **C#命名约定**: PascalCase for public members | 公共成员使用PascalCase
- **Comment Standards** | **注释标准**: XML documentation for public APIs | 公共API使用XML文档
- **File Organization** | **文件组织**: Logical folder structure by functionality | 按功能逻辑文件夹结构

### Architecture Principles | 架构原则
- **Separation of Concerns** | **关注点分离**: UI, game logic, and data are separate | UI、游戏逻辑和数据分离
- **Event-Driven Design** | **事件驱动设计**: Managers communicate via events | 管理器通过事件通信
- **Data-Driven Content** | **数据驱动内容**: Game content defined in JSON files | 游戏内容在JSON文件中定义
- **Dependency Injection** | **依赖注入**: Loose coupling through DI container | 通过DI容器松耦合
- **Interface Segregation** | **接口隔离**: Small, focused interfaces | 小而专注的接口
- **Single Responsibility** | **单一职责**: Each manager has a specific purpose | 每个管理器都有特定目的

### Event-Driven Patterns | 事件驱动模式
- **Publisher-Subscriber** | **发布者-订阅者**: Loose coupling between components | 组件之间的松耦合
- **Command Pattern** | **命令模式**: Encapsulated game actions | 封装的游戏动作
- **Observer Pattern** | **观察者模式**: UI updates via events | 通过事件更新UI
- **Mediator Pattern** | **中介者模式**: EventManager coordinates communication | EventManager协调通信

### Contributing | 贡献
1. **Fork the repository** | **分叉仓库**
2. **Create a feature branch** | **创建功能分支**: `git checkout -b feature/new-feature`
3. **Follow coding standards** | **遵循编码标准**
4. **Write tests for new features** | **为新功能编写测试**
5. **Ensure event-driven design** | **确保事件驱动设计**
6. **Submit a pull request** | **提交拉取请求**

---

## 🎨 Customization | 自定义

### Themes | 主题
- **Classic** | **经典**: Traditional green-on-black terminal | 传统绿字黑底终端
- **Modern** | **现代**: Clean white background | 干净的白色背景
- **Fantasy** | **奇幻**: Mystical purple and gold | 神秘的紫色和金色
- **Dark** | **黑暗**: Easy on the eyes dark theme | 护眼的黑暗主题
- **High Contrast** | **高对比度**: Accessibility-focused design | 注重无障碍的设计

### Modding Support | 模组支持
- **JSON Data Files** | **JSON数据文件**: Easily modify game content | 轻松修改游戏内容
- **Asset Editor** | **资源编辑器**: Visual tool for content creation | 内容创建的可视化工具
- **Custom Skills** | **自定义技能**: Add new abilities and effects | 添加新能力和效果
- **New Locations** | **新地点**: Expand the game world | 扩展游戏世界
- **Event-Based Plugins** | **基于事件的插件**: Extend functionality via events (V3.0) | 通过事件扩展功能 (V3.0)

---

## 🔧 Technical Specifications | 技术规格

### Technologies Used | 使用技术
- **.NET 9.0** | **.NET 9.0**
- **WinForms** | **WinForms**
- **Microsoft.Extensions.Hosting** | **微软扩展主机**: .NET Generic Host for dependency injection | .NET通用主机用于依赖注入
- **Microsoft.Extensions.Logging** | **微软扩展日志**: Structured logging throughout the application | 整个应用程序的结构化日志
- **Microsoft.Extensions.DependencyInjection** | **微软扩展依赖注入**: Service container and lifetime management | 服务容器和生命周期管理
- **System.Text.Json** for data serialization | **System.Text.Json** 用于数据序列化
- **TreeView** for hierarchical data display | **TreeView** 用于分层数据显示
- **Custom UserControls** for modular UI | **自定义用户控件** 用于模块化UI
- **Event-Driven Architecture** with 25+ event types | **事件驱动架构** 包含25+种事件类型
- **Manager Pattern** with specialized business logic managers | **管理器模式** 包含专门的业务逻辑管理器

### Architecture Patterns | 架构模式
- **Event-Driven Architecture** | **事件驱动架构**: Complete decoupling through events | 通过事件完全解耦
- **Dependency Injection** | **依赖注入**: .NET Generic Host container | .NET通用主机容器
- **Manager Pattern** | **管理器模式**: Specialized managers for different concerns | 不同关注点的专门管理器
- **Decorator Pattern** | **装饰器模式**: Logging and validation decorators | 日志和验证装饰器
- **Service Locator** | **服务定位器**: GameCoordinatorService for complex operations | 复杂操作的游戏协调服务
- **Command Pattern** | **命令模式**: Encapsulated game commands | 封装的游戏命令
- **Observer Pattern** | **观察者模式**: Event-based UI updates | 基于事件的UI更新

### Performance | 性能
- **Memory Efficient** | **内存高效**: Optimized for minimal resource usage | 优化最小资源使用
- **Fast Loading** | **快速加载**: JSON data cached for performance | JSON数据缓存以提高性能
- **Responsive UI** | **响应式UI**: Async operations prevent blocking | 异步操作防止阻塞
- **Event Optimization** | **事件优化**: Efficient event routing and filtering | 高效的事件路由和过滤
- **Lazy Loading** | **延迟加载**: Managers initialized on demand | 按需初始化管理器
- **Thread Safety** | **线程安全**: Thread-safe event publishing | 线程安全的事件发布

### Code Quality | 代码质量
- **SOLID Principles** | **SOLID原则**: Single responsibility, open/closed, etc. | 单一职责、开闭原则等
- **Clean Architecture** | **清洁架构**: Separation of concerns and dependency inversion | 关注点分离和依赖倒置
- **Interface Segregation** | **接口隔离**: Small, focused interfaces | 小而专注的接口
- **Dependency Inversion** | **依赖倒置**: Depend on abstractions, not concretions | 依赖抽象而非具体实现
- **Event Sourcing** | **事件溯源**: All state changes through events | 所有状态变化通过事件
- **Testability** | **可测试性**: Interface-based design for easy mocking | 基于接口的设计便于模拟

---

## 📚 Documentation | 文档

### Available Documents | 可用文档
- **[README.md](docs/README.md)** | **[README.md](docs/README.md)**: Main project documentation | 主要项目文档
- **[REFACTORING_PLAN.md](docs/REFACTORING_PLAN.md)** | **[REFACTORING_PLAN.md](docs/REFACTORING_PLAN.md)**: Event-driven architecture migration plan | 事件驱动架构迁移计划
- **[REFACTORING_PROGRESS.md](docs/REFACTORING_PROGRESS.md)** | **[REFACTORING_PROGRESS.md](docs/REFACTORING_PROGRESS.md)**: Progress tracking for V3.0 refactoring | V3.0重构进度跟踪

### Architecture Documentation | 架构文档
- **Event System** | **事件系统**: Complete guide to the event-driven architecture | 事件驱动架构完整指南
- **Manager Pattern** | **管理器模式**: How the 6 specialized managers work together | 6个专门管理器如何协同工作
- **Dependency Injection** | **依赖注入**: .NET Generic Host implementation details | .NET通用主机实现详情
- **GameCoordinatorService** | **游戏协调服务**: Advanced multi-manager operations | 高级多管理器操作

### Code Examples | 代码示例
- **Creating Custom Events** | **创建自定义事件**: How to add new event types | 如何添加新事件类型
- **Manager Implementation** | **管理器实现**: Building new managers | 构建新管理器
- **Service Registration** | **服务注册**: Adding services to DI container | 向DI容器添加服务
- **Advanced Commands** | **高级命令**: Implementing complex game operations | 实现复杂游戏操作

### Upcoming Documentation | 即将推出的文档
- **API Reference** | **API参考**: Complete manager and event documentation | 完整的管理器和事件文档
- **Plugin Development Guide** | **插件开发指南**: How to create mods and extensions | 如何创建模组和扩展
- **Testing Guide** | **测试指南**: Unit and integration testing patterns | 单元和集成测试模式
- **Performance Optimization** | **性能优化**: Best practices for game performance | 游戏性能最佳实践

---

## 📞 Support & Community | 支持与社区

### Getting Help | 获取帮助
- **In-Game Help** | **游戏内帮助**: Press F1 or type `help` | 按F1或输入 `help`
- **Advanced Menu** | **高级菜单**: Use Advanced → Validate Game State for troubleshooting | 使用高级→验证游戏状态进行故障排除
- **GitHub Issues** | **GitHub问题**: Report bugs and request features | 报告错误和请求功能
- **Documentation** | **文档**: Comprehensive guides and tutorials | 全面的指南和教程

### Community | 社区
- **Discord Server** | **Discord服务器**: Join our community discussions | 加入我们的社区讨论
- **Reddit** | **Reddit**: r/RealmOfAethermoor for tips and tricks | r/RealmOfAethermoor 获取技巧和窍门
- **YouTube** | **YouTube**: Video tutorials and gameplay | 视频教程和游戏玩法

### Contributing to Architecture | 架构贡献
- **Event System** | **事件系统**: Propose new event types or improvements | 提议新事件类型或改进
- **Manager Extensions** | **管理器扩展**: Suggest new manager functionality | 建议新管理器功能
- **Performance Improvements** | **性能改进**: Optimize event handling or DI patterns | 优化事件处理或DI模式
- **Documentation** | **文档**: Help improve architecture documentation | 帮助改进架构文档

---

## 📄 License | 许可证

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

本项目采用MIT许可证 - 详情请查看 [LICENSE](LICENSE) 文件。

---

## 🙏 Acknowledgments | 致谢

- **Classic RPG Games** | **经典RPG游戏**: Inspired by timeless text adventures | 受永恒文字冒险启发
- **.NET Community** | **.NET社区**: For excellent documentation and support | 提供优秀文档和支持
- **Microsoft** | **微软**: For the excellent .NET Generic Host and dependency injection framework | 提供优秀的.NET通用主机和依赖注入框架
- **Event-Driven Architecture Pioneers** | **事件驱动架构先驱**: For inspiring clean, maintainable code patterns | 启发干净、可维护的代码模式
- **Contributors** | **贡献者**: Thank you to all who helped improve this project | 感谢所有帮助改进此项目的人

---

**Happy Adventuring!** | **祝您冒险愉快！** 🗡️✨

*Experience the power of modern architecture in classic RPG gaming!*
*在经典RPG游戏中体验现代架构的力量！*

---

*Last updated: December 2024 | 最后更新：2024年12月* 
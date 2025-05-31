# Realm of Aethermoor | 艾瑟摩尔王国
### A Classic Text-Based RPG Adventure | 经典文字冒险RPG游戏

**Version 2.0** | **版本 2.0**

![WinForms](https://img.shields.io/badge/WinForms-.NET%209.0-blue) ![C#](https://img.shields.io/badge/C%23-Latest-green) ![License](https://img.shields.io/badge/License-MIT-yellow)

---

## 📖 Overview | 概述

Realm of Aethermoor is a modern take on classic text-based RPG adventures, built with .NET WinForms. Experience a rich fantasy world where your choices shape your destiny through immersive storytelling, strategic combat, and character progression.

艾瑟摩尔王国是一款基于经典文字冒险RPG的现代化游戏，使用 .NET WinForms 构建。通过沉浸式的故事叙述、策略性战斗和角色成长，体验一个丰富的奇幻世界，在这里您的选择决定命运。

---

## ✨ Features | 特色功能

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
- **Random Encounters** | **随机遭遇**: 30% chance of combat when traveling | 旅行时30%的战斗几乎率

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

### 🛠️ **Asset Editor** | **资源编辑器** (NEW! | 新功能!)
- **TreeView-Based JSON Editor** | **基于TreeView的JSON编辑器**: Visual editing of game data | 游戏数据的可视化编辑
- **Real-time Validation** | **实时验证**: Type checking and data integrity | 类型检查和数据完整性
- **Add/Delete Nodes** | **添加/删除节点**: Modify game content easily | 轻松修改游戏内容
- **Professional Interface** | **专业界面**: Color-coded icons and hierarchical display | 彩色编码图标和分层显示

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
│   │   └── GameEngine.cs        # Core game engine | 核心游戏引擎
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
├── docs/                         # Documentation | 文档
│   └── README.md                # This file | 本文件
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

### 🚧 In Progress | 进行中
- [ ] Enhanced combat animations | 增强战斗动画
- [ ] Quest system implementation | 任务系统实现
- [ ] Advanced NPC interactions | 高级NPC交互

### 📋 Planned Features | 计划功能
- [ ] Multiplayer support | 多人游戏支持
- [ ] Plugin system for mods | 模组插件系统
- [ ] Voice narration | 语音叙述
- [ ] Achievement system | 成就系统
- [ ] Data validation tool | 数据验证工具
- [ ] Import/Export game data | 导入/导出游戏数据

---

## 🛠️ Development Guidelines | 开发指南

### Code Style | 代码风格
- **C# Naming Conventions** | **C#命名约定**: PascalCase for public members | 公共成员使用PascalCase
- **Comment Standards** | **注释标准**: XML documentation for public APIs | 公共API使用XML文档
- **File Organization** | **文件组织**: Logical folder structure by functionality | 按功能逻辑文件夹结构

### Architecture Principles | 架构原则
- **Separation of Concerns** | **关注点分离**: UI, game logic, and data are separate | UI、游戏逻辑和数据分离
- **Event-Driven Design** | **事件驱动设计**: Custom controls communicate via events | 自定义控件通过事件通信
- **Data-Driven Content** | **数据驱动内容**: Game content defined in JSON files | 游戏内容在JSON文件中定义

### Contributing | 贡献
1. **Fork the repository** | **分叉仓库**
2. **Create a feature branch** | **创建功能分支**: `git checkout -b feature/new-feature`
3. **Follow coding standards** | **遵循编码标准**
4. **Write tests for new features** | **为新功能编写测试**
5. **Submit a pull request** | **提交拉取请求**

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

---

## 🔧 Technical Specifications | 技术规格

### Technologies Used | 使用技术
- **.NET 9.0** | **.NET 9.0**
- **WinForms** | **WinForms**
- **System.Text.Json** for data serialization | **System.Text.Json** 用于数据序列化
- **TreeView** for hierarchical data display | **TreeView** 用于分层数据显示
- **Custom UserControls** for modular UI | **自定义用户控件** 用于模块化UI

### Performance | 性能
- **Memory Efficient** | **内存高效**: Optimized for minimal resource usage | 优化最小资源使用
- **Fast Loading** | **快速加载**: JSON data cached for performance | JSON数据缓存以提高性能
- **Responsive UI** | **响应式UI**: Async operations prevent blocking | 异步操作防止阻塞

---

## 📞 Support & Community | 支持与社区

### Getting Help | 获取帮助
- **In-Game Help** | **游戏内帮助**: Press F1 or type `help` | 按F1或输入 `help`
- **GitHub Issues** | **GitHub问题**: Report bugs and request features | 报告错误和请求功能
- **Documentation** | **文档**: Comprehensive guides and tutorials | 全面的指南和教程

### Community | 社区
- **Discord Server** | **Discord服务器**: Join our community discussions | 加入我们的社区讨论
- **Reddit** | **Reddit**: r/RealmOfAethermoor for tips and tricks | r/RealmOfAethermoor 获取技巧和窍门
- **YouTube** | **YouTube**: Video tutorials and gameplay | 视频教程和游戏玩法

---

## 📄 License | 许可证

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

本项目采用MIT许可证 - 详情请查看 [LICENSE](LICENSE) 文件。

---

## 🙏 Acknowledgments | 致谢

- **Classic RPG Games** | **经典RPG游戏**: Inspired by timeless text adventures | 受永恒文字冒险启发
- **.NET Community** | **.NET社区**: For excellent documentation and support | 提供优秀文档和支持
- **Contributors** | **贡献者**: Thank you to all who helped improve this project | 感谢所有帮助改进此项目的人

---

**Happy Adventuring!** | **祝您冒险愉快！** 🗡️✨

---

*Last updated: December 2024 | 最后更新：2024年12月* 
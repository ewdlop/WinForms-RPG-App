# Event-Driven Architecture Refactoring Plan
## Realm of Aethermoor - Version 3.0

### 📋 Overview

This document outlines the comprehensive refactoring plan to transition the Realm of Aethermoor codebase from its current tightly-coupled architecture to a modern event-driven architecture using event handlers and specialized managers.

### 🎯 Goals

1. **Decouple Components**: Reduce direct dependencies between UI, game logic, and data layers
2. **Improve Maintainability**: Make the codebase easier to extend and modify
3. **Enhance Testability**: Enable better unit testing through dependency injection
4. **Increase Scalability**: Support future features like multiplayer, plugins, and mods
5. **Better Separation of Concerns**: Each manager handles a specific domain

---

## 🏗️ Current Architecture Issues

### Problems Identified:
- **Tight Coupling**: `GameEngine` directly manipulates UI controls
- **Monolithic Design**: `GameEngine.cs` (1527 lines) handles too many responsibilities
- **Direct Dependencies**: Forms directly call game engine methods
- **No Event System**: Changes propagate through direct method calls
- **Hard to Test**: Business logic mixed with UI logic

---

## 🎨 Target Architecture

### Event-Driven Architecture with Managers

```
┌─────────────────┐    Events    ┌─────────────────┐
│   UI Layer      │◄────────────►│  Event Bus      │
│  (Forms/Controls)│              │  (EventManager) │
└─────────────────┘              └─────────────────┘
                                          ▲
                                          │ Events
                                          ▼
┌─────────────────────────────────────────────────────────┐
│                Manager Layer                            │
├─────────────────┬─────────────────┬─────────────────────┤
│  GameManager    │  CombatManager  │  InventoryManager   │
│  PlayerManager  │  LocationManager│  SaveManager        │
│  ItemManager    │  SkillManager   │  SettingsManager    │
└─────────────────┴─────────────────┴─────────────────────┘
                          ▲
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                Data Layer                               │
├─────────────────┬─────────────────┬─────────────────────┤
│   Models        │   DataLoader    │   Repositories      │
│   (Entities)    │   (JSON/DB)     │   (Data Access)     │
└─────────────────┴─────────────────┴─────────────────────┘
```

---

## 📦 Manager Breakdown

### 1. **EventManager** (Central Event Bus)
**Responsibilities:**
- Centralized event publishing and subscription
- Event routing and filtering
- Event history and debugging

**Events to Handle:**
- `PlayerStatsChanged`
- `InventoryUpdated`
- `LocationChanged`
- `CombatStarted/Ended`
- `ItemUsed/Equipped`
- `SkillLearned`
- `GameSaved/Loaded`

### 2. **GameManager** (Game State & Flow)
**Responsibilities:**
- Overall game state management
- Game initialization and cleanup
- Save/Load coordination
- Cheat system management

**Key Methods:**
- `StartNewGame()`
- `SaveGame(string saveName)`
- `LoadGame(string saveName)`
- `ProcessCommand(string command)`

### 3. **PlayerManager** (Player State)
**Responsibilities:**
- Player stats and progression
- Experience and leveling
- Character class management

**Events Published:**
- `PlayerLeveledUp`
- `PlayerStatsChanged`
- `PlayerHealthChanged`
- `PlayerDied`

### 4. **CombatManager** (Combat System)
**Responsibilities:**
- Combat state management
- Turn-based combat logic
- Damage calculations
- Combat rewards

**Events Published:**
- `CombatStarted`
- `CombatEnded`
- `PlayerAttacked`
- `EnemyDefeated`

### 5. **InventoryManager** (Items & Equipment)
**Responsibilities:**
- Inventory operations
- Equipment management
- Item usage logic

**Events Published:**
- `InventoryUpdated`
- `ItemAdded/Removed`
- `ItemEquipped/Unequipped`
- `ItemUsed`

### 6. **LocationManager** (World & Movement)
**Responsibilities:**
- Location management
- Movement validation
- Random encounters
- Location state

**Events Published:**
- `LocationChanged`
- `RandomEncounterTriggered`
- `LocationDiscovered`

### 7. **SkillManager** (Skills & Abilities)
**Responsibilities:**
- Skill tree management
- Skill learning and validation
- Skill point allocation

**Events Published:**
- `SkillLearned`
- `SkillPointsChanged`
- `SkillTreeUpdated`

### 8. **SaveManager** (Persistence)
**Responsibilities:**
- Save file management
- Data serialization
- Auto-save functionality

**Events Published:**
- `GameSaved`
- `GameLoaded`
- `SaveError`

---

## 🔄 Refactoring Phases

### Phase 1: Foundation (Week 1-2)
**Priority: High**

1. **Create Event System**
   - Implement `EventManager` class
   - Define base event classes
   - Create event argument classes

2. **Extract Manager Interfaces**
   - Define `IGameManager`, `IPlayerManager`, etc.
   - Create base manager abstract class
   - Implement dependency injection container

3. **Create Core Managers**
   - `GameManager` (extract from `GameEngine`)
   - `EventManager` (new)
   - `PlayerManager` (extract player logic)

### Phase 2: Core Managers (Week 3-4)
**Priority: High**

1. **Implement Remaining Managers**
   - `CombatManager`
   - `InventoryManager`
   - `LocationManager`
   - `SkillManager`

2. **Refactor GameEngine**
   - Remove direct UI manipulation
   - Delegate to appropriate managers
   - Implement event publishing

### Phase 3: UI Refactoring (Week 5-6)
**Priority: Medium**

1. **Update Forms to Use Events**
   - Subscribe to relevant events
   - Remove direct GameEngine calls
   - Implement event handlers

2. **Enhance Custom Controls**
   - Make controls event-driven
   - Remove tight coupling to forms
   - Implement proper data binding

### Phase 4: Advanced Features (Week 7-8)
**Priority: Low**

1. **Add Advanced Event Features**
   - Event filtering and routing
   - Event history and replay
   - Performance monitoring

2. **Implement Plugin Architecture**
   - Plugin manager
   - Event-based plugin system
   - Mod support framework

---

## 📝 Implementation Details

### Event System Design

```csharp
// Base event class
public abstract class GameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string EventId { get; } = Guid.NewGuid().ToString();
}

// Specific event examples
public class PlayerStatsChangedEvent : GameEvent
{
    public Player Player { get; set; }
    public string StatName { get; set; }
    public int OldValue { get; set; }
    public int NewValue { get; set; }
}

public class InventoryUpdatedEvent : GameEvent
{
    public List<Item> Items { get; set; }
    public InventoryAction Action { get; set; }
    public Item AffectedItem { get; set; }
}

// Event manager interface
public interface IEventManager
{
    void Subscribe<T>(Action<T> handler) where T : GameEvent;
    void Unsubscribe<T>(Action<T> handler) where T : GameEvent;
    void Publish<T>(T gameEvent) where T : GameEvent;
}
```

### Manager Base Class

```csharp
public abstract class BaseManager
{
    protected IEventManager EventManager { get; }
    protected ILogger Logger { get; }

    protected BaseManager(IEventManager eventManager, ILogger logger)
    {
        EventManager = eventManager;
        Logger = logger;
        SubscribeToEvents();
    }

    protected abstract void SubscribeToEvents();
    public abstract void Initialize();
    public abstract void Cleanup();
}
```

### Dependency Injection Setup

```csharp
public class GameServiceContainer
{
    private readonly Dictionary<Type, object> _services = new();

    public void RegisterSingleton<T>(T instance)
    {
        _services[typeof(T)] = instance;
    }

    public T GetService<T>()
    {
        return (T)_services[typeof(T)];
    }
}
```

---

## 🧪 Testing Strategy

### Unit Testing Approach
1. **Manager Testing**: Each manager can be tested in isolation
2. **Event Testing**: Mock event manager for testing event publishing
3. **Integration Testing**: Test manager interactions through events
4. **UI Testing**: Mock managers for UI component testing

### Test Structure
```
Tests/
├── Unit/
│   ├── Managers/
│   │   ├── PlayerManagerTests.cs
│   │   ├── CombatManagerTests.cs
│   │   └── InventoryManagerTests.cs
│   └── Events/
│       └── EventManagerTests.cs
├── Integration/
│   ├── ManagerInteractionTests.cs
│   └── EventFlowTests.cs
└── UI/
    ├── FormTests.cs
    └── ControlTests.cs
```

---

## 📊 Migration Checklist

### Phase 1 Checklist
- [ ] Create `EventManager` class
- [ ] Define base event classes
- [ ] Create `GameManager` interface and implementation
- [ ] Extract player logic to `PlayerManager`
- [ ] Set up dependency injection container
- [ ] Create unit tests for core managers

### Phase 2 Checklist
- [ ] Implement `CombatManager`
- [ ] Implement `InventoryManager`
- [ ] Implement `LocationManager`
- [ ] Implement `SkillManager`
- [ ] Refactor `GameEngine` to use managers
- [ ] Update event publishing throughout

### Phase 3 Checklist
- [ ] Update `Form1` to use events
- [ ] Update `InventoryForm` to use events
- [ ] Update `SkillTreeForm` to use events
- [ ] Update `MapForm` to use events
- [ ] Refactor custom controls
- [ ] Remove direct GameEngine dependencies

### Phase 4 Checklist
- [ ] Add event filtering and routing
- [ ] Implement event history
- [ ] Create plugin manager
- [ ] Add performance monitoring
- [ ] Documentation updates

---

## 🚀 Benefits After Refactoring

### For Developers
- **Easier Testing**: Isolated components with clear interfaces
- **Better Debugging**: Event history and tracing
- **Faster Development**: Reusable managers and clear separation
- **Plugin Support**: Easy to extend with new features

### For Users
- **Better Performance**: Optimized event handling
- **More Stable**: Reduced coupling means fewer bugs
- **Extensible**: Support for mods and plugins
- **Better UX**: More responsive UI through async events

---

## 📚 Resources and References

### Design Patterns Used
- **Observer Pattern**: Event system implementation
- **Mediator Pattern**: EventManager as central mediator
- **Strategy Pattern**: Different combat strategies
- **Command Pattern**: Game command processing
- **Repository Pattern**: Data access abstraction

### External Libraries to Consider
- **Microsoft.Extensions.DependencyInjection**: For DI container
- **Microsoft.Extensions.Logging**: For logging
- **Newtonsoft.Json**: For enhanced JSON handling
- **AutoMapper**: For object mapping

---

## 🔄 Rollback Plan

### If Issues Arise
1. **Feature Flags**: Use flags to toggle between old and new systems
2. **Gradual Migration**: Keep old GameEngine as fallback
3. **Version Control**: Maintain separate branches for each phase
4. **Testing Gates**: Don't proceed to next phase without passing tests

### Risk Mitigation
- **Backup Strategy**: Full backup before each phase
- **Incremental Deployment**: Deploy one manager at a time
- **User Testing**: Beta testing with select users
- **Performance Monitoring**: Track performance metrics

---

*Last Updated: [Current Date]*
*Version: 1.0*
*Status: Planning Phase* 
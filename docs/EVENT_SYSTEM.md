# Event Handler Pattern Implementation

## Overview

The Realm of Aethermoor RPG game implements a comprehensive event-driven architecture using the Event Handler pattern. This system provides loose coupling between game components, enabling extensible functionality for logging, achievements, UI updates, and analytics.

## Architecture Components

### 1. Event Arguments Classes (`src/Events/GameEvents.cs`)

Custom event argument classes that inherit from `EventArgs` and carry specific data:

- **PlayerEventArgs**: Base class for player-related events
- **LevelUpEventArgs**: Player level progression with stat gains
- **ExperienceGainEventArgs**: Experience point changes
- **GoldChangeEventArgs**: Currency modifications with reasons
- **HealthChangeEventArgs**: Health modifications with context
- **CombatEventArgs**: Combat-related events
- **CombatActionEventArgs**: Individual combat actions with damage/critical info
- **CombatEndEventArgs**: Combat resolution with rewards
- **LocationChangeEventArgs**: Player movement between locations
- **InventoryChangeEventArgs**: Item additions/removals
- **ItemUsedEventArgs**: Item consumption and effects
- **SkillEventArgs**: Skill learning and progression
- **CheatEventArgs**: Cheat code activation tracking
- **GameStateEventArgs**: General game state changes
- **SaveGameEventArgs**: Save operation results
- **LoadGameEventArgs**: Load operation results
- **CommandEventArgs**: Command execution tracking
- **MessageEventArgs**: Message display with type categorization

### 2. Event Manager (`src/Events/GameEventManager.cs`)

Central event hub implementing the Singleton pattern:

```csharp
public class GameEventManager : IGameEventPublisher
{
    public static GameEventManager Instance { get; }
    
    // Event declarations
    public event EventHandler<LevelUpEventArgs>? PlayerLeveledUp;
    public event EventHandler<CombatEventArgs>? CombatStarted;
    // ... more events
    
    // Publishing methods
    public void PublishPlayerLevelUp(Player player, int oldLevel, int newLevel, ...);
    public void PublishCombatStart(Player player, Enemy enemy);
    // ... more publishers
}
```

**Key Features:**
- Thread-safe singleton implementation
- Type-safe event publishing methods
- Subscription helper methods for bulk event registration
- Event statistics and debugging capabilities
- Automatic message publishing for user feedback

### 3. Event Handler Interfaces

Type-safe interfaces for event subscription:

```csharp
public interface IPlayerEventHandler
{
    void OnPlayerLeveledUp(object? sender, LevelUpEventArgs e);
    void OnPlayerGainedExperience(object? sender, ExperienceGainEventArgs e);
    // ... more handlers
}

public interface ICombatEventHandler
{
    void OnCombatStarted(object? sender, CombatEventArgs e);
    void OnCombatActionPerformed(object? sender, CombatActionEventArgs e);
    void OnCombatEnded(object? sender, CombatEndEventArgs e);
}
```

### 4. Event Subscribers

#### Form1 (UI Handler)
Implements multiple event handler interfaces for UI updates:

```csharp
public partial class Form1 : Form, IMessageEventHandler, IPlayerEventHandler, 
    ICombatEventHandler, IGameStateEventHandler
{
    public void OnPlayerLeveledUp(object? sender, LevelUpEventArgs e)
    {
        UpdateCharacterDisplay();
        ShowLevelUpNotification(e.OldLevel, e.NewLevel);
    }
    
    public void OnCombatActionPerformed(object? sender, CombatActionEventArgs e)
    {
        if (e.IsCritical)
            FlashCriticalHit();
    }
}
```

#### GameEventLogger (Analytics & Achievements)
Comprehensive event logging and achievement system:

```csharp
public class GameEventLogger : IPlayerEventHandler, ICombatEventHandler, 
    IWorldEventHandler, IGameStateEventHandler
{
    public void OnPlayerLeveledUp(object? sender, LevelUpEventArgs e)
    {
        LogEvent("PlayerLevelUp", eventData);
        CheckLevelAchievements(e.NewLevel);
    }
}
```

## Event Flow Examples

### 1. Player Level Up Sequence

```
1. GameEngine.LevelUp() called
2. Player stats updated
3. eventManager.PublishPlayerLevelUp() called
4. Events fired to all subscribers:
   - Form1.OnPlayerLeveledUp() → UI updates, level up animation
   - GameEventLogger.OnPlayerLeveledUp() → Logging, achievement check
5. Automatic message published for user feedback
```

### 2. Combat Action Sequence

```
1. GameEngine.PerformAttack() called
2. Damage calculated, critical hit determined
3. eventManager.PublishCombatAction() called
4. Events fired:
   - Form1.OnCombatActionPerformed() → Screen flash for critical
   - GameEventLogger.OnCombatActionPerformed() → Combat statistics
5. Health change events triggered if damage dealt
```

### 3. Item Usage Sequence

```
1. GameEngine.UseItem() called
2. Item effect applied (health restoration, etc.)
3. Multiple events published:
   - eventManager.PublishItemUsed()
   - eventManager.PublishHealthChange() (if healing item)
   - eventManager.PublishInventoryChange() (if consumed)
4. UI updates and logging occur automatically
```

## Benefits of This Implementation

### 1. Loose Coupling
- GameEngine doesn't need direct references to UI or logging components
- New event subscribers can be added without modifying existing code
- Components can be easily swapped or removed

### 2. Extensibility
- New event types can be added without breaking existing functionality
- Multiple subscribers can handle the same event differently
- Easy to add new features like achievements, analytics, or debugging tools

### 3. Separation of Concerns
- Game logic focuses on game state management
- UI components handle presentation and user interaction
- Logging components handle data persistence and analytics
- Each component has a single responsibility

### 4. Testability
- Events can be easily mocked for unit testing
- Event publishing can be verified independently
- Components can be tested in isolation

### 5. Performance
- Events are only fired when something actually happens
- Subscribers can choose which events to handle
- No polling or constant checking required

## Usage Examples

### Adding a New Event Subscriber

```csharp
public class SoundManager : ICombatEventHandler
{
    public SoundManager()
    {
        GameEventManager.Instance.SubscribeToCombatEvents(this);
    }
    
    public void OnCombatStarted(object? sender, CombatEventArgs e)
    {
        PlaySound("combat_start.wav");
    }
    
    public void OnCombatActionPerformed(object? sender, CombatActionEventArgs e)
    {
        if (e.IsCritical)
            PlaySound("critical_hit.wav");
        else
            PlaySound("attack.wav");
    }
    
    public void OnCombatEnded(object? sender, CombatEndEventArgs e)
    {
        PlaySound(e.PlayerWon ? "victory.wav" : "defeat.wav");
    }
}
```

### Publishing Custom Events

```csharp
// In GameEngine or other components
eventManager.PublishMessage("Custom event occurred!", Color.Blue, MessageType.System);
eventManager.PublishGameStateChange("CustomState", customData);
```

### Event Statistics and Debugging

```csharp
// Get subscription counts for debugging
var stats = eventManager.GetEventSubscriptionCounts();
foreach (var kvp in stats)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
}

// Get event log from logger
var logger = new GameEventLogger();
var events = logger.GetEventLog();
var achievements = logger.GetAchievements();
```

## Best Practices

### 1. Event Naming
- Use descriptive event names that clearly indicate what happened
- Follow consistent naming patterns (e.g., "PlayerLeveledUp", "CombatStarted")
- Use past tense for completed actions

### 2. Event Data
- Include all relevant information in event arguments
- Make event arguments immutable when possible
- Provide context information (timestamps, reasons, etc.)

### 3. Error Handling
- Event handlers should not throw exceptions that break the event chain
- Log errors within handlers rather than propagating them
- Use try-catch blocks in critical event handlers

### 4. Performance
- Keep event handlers lightweight and fast
- Avoid heavy processing in event handlers
- Consider async patterns for long-running operations

### 5. Memory Management
- Always unsubscribe from events when objects are disposed
- Use weak references for long-lived event subscriptions if needed
- Be careful with anonymous delegates and closures

## Integration with Game Systems

### Cheat System Integration
```csharp
// Cheat activation automatically publishes events
eventManager.PublishCheatActivated("godmode", args, player);
eventManager.PublishMessage("God Mode activated!", Color.Magenta, MessageType.Cheat);
```

### Save/Load System Integration
```csharp
// Save operations publish success/failure events
eventManager.PublishGameSaved(saveName, savePath, true);
eventManager.PublishGameLoaded(saveName, savePath, true, loadedPlayer);
```

### Achievement System Integration
```csharp
// Automatic achievement checking based on events
private void CheckLevelAchievements(int level)
{
    if (level == 10)
        LogAchievement("Experienced Explorer", "Reached level 10");
}
```

This event system provides a robust foundation for game development that can easily accommodate new features, debugging tools, and analytics without requiring changes to core game logic. 
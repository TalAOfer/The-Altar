using System;

[Serializable]
public class Level 
{
    public Codex PlayerCodex { get; private set; }
    public Codex EnemyCodex { get; private set; }

    public Deck PlayerDeck;
}

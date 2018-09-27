public enum ColorblindMode { NONE, RedGreen, Other }
public enum Units { NONE, Inches, Feet, Centimeters, Meters }
public enum Sex { NONE, Male, Female, Other, Undisclosed }
public enum Path2DTileType { NONE, walkable, unwalkable }
public enum ConnectionType { ALL, connectionTestA, connectionTestB }
public enum GenerationType { BFS, DFS, Combination }
public enum StatType { points, healthCurrent, healthMax, movementSpeed, baseDamage, reviveCount, stalenessMeter }
public enum Rarity { common, uncommon, rare, epic, legendary, artifact }
public enum PlayerState { alive, incapacitated, dead, disconnected }
public enum PositionToSpawn { ALL, behind, infront }
public enum SpawnType { playerAccessable, playerUnaccessable }
public enum Thing { weapon, ability, ai, levelPieceGeneral }   // TODO rename
public enum MeleeAttackMode { NONE, swing, stab }
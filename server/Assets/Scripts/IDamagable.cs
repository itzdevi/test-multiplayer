public interface IDamagable {
    int GetHealth();
    void Damage(int amount);
    void Heal(int amount);
}
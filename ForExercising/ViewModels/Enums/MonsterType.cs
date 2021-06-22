using System.ComponentModel.DataAnnotations;

namespace ForExercising.ViewModels.Enums
{
    public enum MonsterType
    {
        Aqua,
        Beast,
        [Display(Name ="Beast-Warrior")]BeastWarrior,
        Cyberse,
        Dinosaur,
        [Display(Name = "Divine-Beast")] DivineBeast,
        Dragon,
        Fairy,
        Fiend,
        Fish,
        Insect,
        Machine,
        Plant,
        Psychic,
        Pyro,
        Reptile,
        Rock,
        [Display(Name = "Sea Serpent")] SeaSerpent,
        Spellcaster,
        Thunder,
        Warrior,
        [Display(Name = "Winged Beast")] WingedBeast,
        Wyrm,
        Zombie
    }
}

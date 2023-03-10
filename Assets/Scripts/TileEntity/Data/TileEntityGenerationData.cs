using UnityEngine;

namespace Klonk.TileEntity.Data
{
    [CreateAssetMenu(fileName = nameof(TileEntityGenerationData), menuName = nameof(TileEntityGenerationData))]
    public class TileEntityGenerationData : ScriptableObject
    {
        public int SandPatchGenerationAmount => _sandPatchGenerationAmount;
        public int SandGenerationAmount => _sandGenerationAmount;
        public int RockPatchGenerationAmount => _rockPatchGenerationAmount;
        public int RockGenerationAmount => _rockGenerationAmount;
        public int RootPatchGenerationAmount => _rootPatchGenerationAmount;
        public int RootGenerationAmount => _rootGenerationAmount;
        public int WaterSpawnSourceAmount => _waterSpawnSourceAmount;
        public int AcidSpawnSourceAmount => _acidSpawnSourceAmount;
        public int GenerationWidth => _generationWidth;
        public int GenerationHeight => _generationHeight;

        [SerializeField] private int _sandPatchGenerationAmount = 10;
        [SerializeField] private int _sandGenerationAmount = 1000;
        [SerializeField] private int _rockPatchGenerationAmount = 10;
        [SerializeField] private int _rockGenerationAmount = 1000;
        [SerializeField] private int _rootPatchGenerationAmount = 10;
        [SerializeField] private int _rootGenerationAmount = 100;
        [SerializeField] private int _waterSpawnSourceAmount = 30;
        [SerializeField] private int _acidSpawnSourceAmount = 10;
        [SerializeField] private int _generationWidth = 1000;
        [SerializeField] private int _generationHeight = 1000;
    }
}
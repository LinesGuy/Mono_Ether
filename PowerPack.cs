using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mono_Ether;
using System;
using System.Diagnostics;

namespace Mono_Ether {
    public enum PowerPackType { MoveSpeedIncrease, MoveSpeedDecrease, ShootSpeedIncrease, ShootSpeedDecrease, Doom }
    public class PowerPack : Entity {
        private static SoundEffect _pickupGoodSound;
        private static SoundEffect _pickupBadSound;
        public static Texture2D _moveSpeedIncreaseTexture;
        public static Texture2D _moveSpeedDecreaseTexture;
        public static Texture2D _shootSpeedIncreaseTexture;
        public static Texture2D _shootSpeedDecreaseTexture;
        public static Texture2D _doomTexture;
        private TimeSpan _timeUntilStart = TimeSpan.FromSeconds(1);
        private bool _isActive => _timeUntilStart <= TimeSpan.Zero;
        public PowerPackType Type;
        public TimeSpan InitialTime; // Used for drawing time remaining on the hud
        public TimeSpan TimeRemaining;
        private TimeSpan _lifeSpan; // When framesExisted is greater than this, the powerup will expire
        private TimeSpan _age;
        public bool IsExpended;
        public bool IsGood; // true = speed increase etc, false = speed decrease etc
        private readonly Random Rand = new Random();
        // Base the texture on the power pack type
        public PowerPack(PowerPackType type, Vector2 position, TimeSpan duration) {
            switch (type) {
                case PowerPackType.MoveSpeedIncrease:
                    IsGood = true;
                    _lifeSpan = TimeSpan.FromSeconds(20);
                    Image = _moveSpeedIncreaseTexture;
                    break;
                case PowerPackType.MoveSpeedDecrease:
                    _lifeSpan = TimeSpan.FromSeconds(10);
                    Image = _moveSpeedDecreaseTexture;
                    IsGood = false;
                    break;
                case PowerPackType.ShootSpeedIncrease:
                    _lifeSpan = TimeSpan.FromSeconds(20);
                    Image = _shootSpeedIncreaseTexture;
                    IsGood = true;
                    break;
                case PowerPackType.ShootSpeedDecrease:
                    _lifeSpan = TimeSpan.FromSeconds(10);
                    Image = _shootSpeedDecreaseTexture;
                    IsGood = false;
                    break;
                case PowerPackType.Doom:
                    _lifeSpan = TimeSpan.FromSeconds(60);
                    Image = _doomTexture;
                    IsGood = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Position = position;
            EntityColor = Color.Transparent;
            Type = type;
            InitialTime = duration;
            TimeRemaining = InitialTime;
        }
        public static void LoadContent(ContentManager content) {
            _pickupGoodSound = content.Load<SoundEffect>("SoundEffects/PowerPacks/PickupGoodSound");
            _pickupBadSound = content.Load<SoundEffect>("SoundEffects/PowerPacks/PickupBadSound");
            _moveSpeedIncreaseTexture = content.Load<Texture2D>("Textures/GameScreen/PowerPacks/MoveSpeedIncrease");
            _moveSpeedDecreaseTexture = content.Load<Texture2D>("Textures/GameScreen/PowerPacks/MoveSpeedDecrease");
            _shootSpeedIncreaseTexture = content.Load<Texture2D>("Textures/GameScreen/PowerPacks/ShootSpeedIncrease");
            _shootSpeedDecreaseTexture = content.Load<Texture2D>("Textures/GameScreen/PowerPacks/ShootSpeedDecrease");
            _doomTexture = content.Load<Texture2D>("Textures/GameScreen/PowerPacks/Doom");
        }
        public static void UnloadContent() {
            _pickupGoodSound = null;
            _pickupBadSound = null;
            _moveSpeedIncreaseTexture = null;
            _moveSpeedDecreaseTexture = null;
            _shootSpeedIncreaseTexture = null;
            _shootSpeedDecreaseTexture = null;
            _doomTexture = null;
        }
        public override void Update(GameTime gameTime) {
            // Fade-in powerup on spawn
            if (_timeUntilStart > TimeSpan.Zero) {
                _timeUntilStart -= gameTime.ElapsedGameTime;
                EntityColor = Color.White * (1 - (float)(_timeUntilStart / TimeSpan.FromSeconds(1)));
            }
            // If powerup is uncollected for lifeSpan frames, powerup expires
            _age += gameTime.ElapsedGameTime;
            if (_age >= _lifeSpan)
                IsExpired = true;
        }
        public void WasPickedUp() {
            Color color;
            if (IsGood) {
                _pickupGoodSound.CreateInstance().Play();
                color = new Color(100, 200, 0); // Green
            } else {
                _pickupBadSound.CreateInstance().Play();
                color = new Color(200, 100, 0); // Red
            }
            ParticleTemplates.Explosion(Position, 1f, 30f, 50, color);
        }
    }
}
public class PowerPackSpawner {
    public static PowerPackSpawner Instance;
    static readonly Random Rand = new Random();
    readonly float InverseSpawnChance = 50;
    public bool Enabled = true;
    public PowerPackSpawner() {
        Instance = this;
    }
    public void Update(GameTime gameTime) {
        if (!Enabled) return;
        if (!EntityManager.Instance.Players.TrueForAll(p => !p.IsDead) ||
            EntityManager.Instance.PowerPacks.Count >= 3) return;
        if (Rand.Next((int)InverseSpawnChance) != 0)
            return;
        /* Get valid spawn position */
        Vector2 spawnPos;
        var playerPos = EntityManager.Instance.Players[Rand.Next(EntityManager.Instance.Players.Count)].Position;
        var remainingAttempts = 10;
        const float radius = 500f;
        do {
            spawnPos = new Vector2(Rand.NextFloat(playerPos.X - radius, playerPos.X + radius),
                Rand.NextFloat(playerPos.Y - radius, playerPos.Y + radius));
            remainingAttempts -= 1;
        }
        while ((Vector2.DistanceSquared(spawnPos, playerPos) < Math.Pow(radius / 2f, 2)
                || TileMap.Instance.GetTileFromMap(TileMap.Instance.WorldtoMap(spawnPos)).Id > 0
                || spawnPos.X < 0 || spawnPos.Y < 0 || spawnPos.X > TileMap.Instance.WorldSize.X || spawnPos.Y > TileMap.Instance.WorldSize.Y)
               && remainingAttempts > 0);

        if (remainingAttempts == 0) {
            Debug.WriteLine("Skipping enemy spawn");
            return;
        }

        int powerTypeInt = Rand.Next(0, 5);
        if (GameScreen.Instance.CurrentLevel != Level.Debug)
            powerTypeInt = Rand.Next(0, 4);
        switch (powerTypeInt) {
            case (0): // ShootSpeedIncrease
                EntityManager.Instance.Add(new PowerPack(PowerPackType.ShootSpeedIncrease, spawnPos, TimeSpan.FromSeconds(5)));
                break;
            case (1): // ShootSpeedDecrease
                EntityManager.Instance.Add(new PowerPack(PowerPackType.ShootSpeedDecrease, spawnPos, TimeSpan.FromSeconds(5)));
                break;
            case (2): // MoveSpeedIncrease
                EntityManager.Instance.Add(new PowerPack(PowerPackType.MoveSpeedIncrease, spawnPos, TimeSpan.FromSeconds(5)));
                break;
            case (3): // MoveSpeedDecrease
                EntityManager.Instance.Add(new PowerPack(PowerPackType.MoveSpeedDecrease, spawnPos, TimeSpan.FromSeconds(5)));
                break;
            case (4): // Doom
                EntityManager.Instance.Add(new PowerPack(PowerPackType.Doom, spawnPos, TimeSpan.FromSeconds(30)));
                break;
            default:
                // this shouldn't happen
                Debug.WriteLine("PowerPack.cs powerTypeInt was unhandled");
                break;
        }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PG.Asteroids.Models.DataModels
{
    [Serializable]
    public class MetaData
    {
        public ShipData ShipData;
        public BulletSettings BulletSettings;
        public AsteroidsData AsteroidsData;
        public ExplosionSettings ExplosionSettings;
        public int Lives;
    }
    
    [Serializable]
    public class ShipData
    {
        public AudioClip DeathSound;
        public float DeathVolume = 1.0f;
        
        public float ShipShieldDuration;
        public float AutoFireDelay;
        public float ShipMaxMovementSpeed;
        public float ShipMovementAcceleration;
        public float ShipDeacceleration;
        public float ShipRotationSpeed;
        public float SpeedForMaxEmisssion;
        public float MaxEmission;
    }
    
    [Serializable]
    public class AsteroidsData
    {
        public int StartingSpawns;
        public int MaxSpawns;
        
        public AsteroidLevelData[]  AsteroidLevels;
    }
    
    [Serializable]
    public class AsteroidLevelData
    {
        public float MinSpeed;
        public float MaxSpeed;
        public float MinScale;
        public float MaxScale;
        public float MinMass;
        public float MaxMass;
        public int HitPoints;
    }
    
    [Serializable]
    public class BulletSettings
    {
        public AudioClip Laser;
        public float LaserVolume = 1.0f;

        public float BulletLifetime;
        public float BulletSpeed;
        public float MaxShootInterval;
        public float BulletOffsetDistance;
    }
    
    [Serializable]
    public class ExplosionSettings
    {
        public AudioClip Explosion;
        public float ExplosionVolume = 1.0f;

        public int ExplosionTimeout;
    }
}
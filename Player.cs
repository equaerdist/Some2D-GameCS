﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using Image = System.Drawing.Image;
using Timer = System.Windows.Forms.Timer;

namespace Game
{
    public class Player
    {
        private  double _absoluteAmountXOffset;
        private  double _absoluteAmountYOffset;
        public  double AbsoluteAmountYOffset
        {
            get
            {
                return _absoluteAmountYOffset;
            }
            set
            {
                if (value>= 0)
                    _absoluteAmountYOffset = value;
                else
                {
                    _absoluteAmountYOffset = 0;
                }
            }
        }
        public double AbsoluteAmountXOffset
        {
            get
            {
                return _absoluteAmountXOffset;
            }
            set
            {
                if (value >= 0)
                    _absoluteAmountXOffset = value;
                else _absoluteAmountXOffset = 0;
            }
        }
        public HealthPoint HP { get; set; }
        public int Energy { get; set; }
        private Func<int, Timer> createTimer;
        public enum Mode { Slow, Fast };
        public bool inAction { get; set; }
        public enum Direction { Left, Right, Up, Down };
        public Direction CurrentDirection { get; set; }
        public Vector Location { get; set; }
        public Weapon CurrentWeapon { get; set; }
        public Vector Velocity { get; set; }
        public Mode SpeedMode { get; set; }
        public enum HealthPoint { VeryLow, Low, Middle, Good, VeryGood};
        public Map CurrentMap { get; }
        public Player(Map map,Weapon weapon)
        {
            HP = HealthPoint.VeryGood;
            createTimer = (time) => { return new Timer() { Interval = time }; };
            AbsoluteAmountXOffset = 0;
            AbsoluteAmountYOffset = 0;
            CurrentMap = map;
            CurrentDirection = Direction.Right;
            CurrentWeapon = weapon;
        }
        public Image CurrentTexture { get; set; }
        public event Action StateChanged;
        private (bool Exists , int Range, Direction direction, Tuple<int, int>) checkObjectsThrowX(Vector currentLocation)
        {
            var map = CurrentMap.GameArea;
            var counter = 1;
            var switcher = CurrentDirection == Direction.Left ? -1 : 1;
            var edge = CurrentDirection == Direction.Left ? 0 : CurrentMap.GameArea.GetLength(0);
            var startY = currentLocation.Y - 42 < 0 ? 0 : currentLocation.Y - 42;
            var endY = currentLocation.Y + 42 >= map.GetLength(0) ? map.GetLength(0) :
                currentLocation.Y + 42;
            while (counter != CurrentWeapon.Range && (int)(currentLocation.X + counter * switcher) != edge)
            {
                for(int i = (int)startY; i < (int)endY;i++)
                {
                    try
                    {
                        if (map[i, (int)(currentLocation.X + counter * switcher)] != (byte)Map.Objects.Grass)
                        {
                            return (true, counter, switcher == -1 ? Direction.Left : Direction.Right, 
                                Tuple.Create(i, (int)(currentLocation.X + counter * switcher)));
                        }
                    }
                    catch
                    {
                        return (false, 0, switcher == -1 ? Direction.Left : Direction.Right, Tuple.Create(0,0));
                    }
                }
                counter++;
            }
            return (false, 0, switcher == -1 ? Direction.Left : Direction.Right, Tuple.Create(0, 0));
        }
        public void MakeShoot(MyFrom window)
        {
            var map = CurrentMap.GameArea;
            var currentLocation = new Vector(Location.X, Location.Y + 15 * window.Height / 600);
            var result = checkObjectsThrowX(currentLocation);
            var relativeDestination = result.Exists ? result.Range : CurrentWeapon.Range;
            var edge = result.direction == Direction.Left ? 0 : map.GetLength(0);
            var switcher = result.direction == Direction.Left ? -1 : 1;
            if (switcher == -1 && (int)(Location.X - relativeDestination) < edge) 
                relativeDestination = (int)Location.X;
            if (switcher == 1 && (int)(Location.X + relativeDestination) > map.GetLength(0)) 
                relativeDestination = (int)(map.GetLength(0) - (int)Location.X);
            var expectedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + relativeDestination / 
                CurrentWeapon.Physic.Velocity.X *
                CurrentWeapon.Physic.VelocityIntervalInMillisecond;
            var lastDate = DateTime.Now;
            var bulletLocationX = currentLocation.X + 4 * switcher;
            while (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond  < expectedTime)
            {
               
                    if (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond > 
                        lastDate.Ticks / TimeSpan.TicksPerMillisecond + CurrentWeapon.Physic.VelocityIntervalInMillisecond)
                    {
                 
                        map[(int)currentLocation.Y, (int)bulletLocationX] = (byte)Map.Objects.Grass;
                        bulletLocationX += CurrentWeapon.Physic.Velocity.X * switcher;
                        map[(int)currentLocation.Y, (int)bulletLocationX] = (byte)Map.Objects.Bullet;
                        lastDate = DateTime.Now;
                        StateChanged();
                    }
            }
            if((int)bulletLocationX >= map.GetLength(0))
                map[(int)currentLocation.Y, (int)(bulletLocationX - 
                 CurrentWeapon.Physic.Velocity.X * switcher)] = (byte)Map.Objects.Grass; 
            else
                map[(int)currentLocation.Y, (int)bulletLocationX] = (byte)Map.Objects.Grass;
            if(result.Exists)
            {

                    if (CurrentMap.Object.ContainsKey(result.Item4))
                    {
                    var damage = CurrentWeapon.Physic.FinallyDamage(result.Range);
                    CurrentMap.Object[result.Item4].HP -= damage;
                        if (CurrentMap.Object[result.Item4].HP == 0)
                        {
                            lock (CurrentMap.Object)
                            {
                                CurrentMap.Object.Remove(result.Item4);
                                CurrentMap.GameArea[result.Item4.Item1, result.Item4.Item2] = (byte)Map.Objects.Grass;
                            }
                        }
                        else
                        {
                            window.BeginInvoke(new Action(() =>
                            {
                                ViewControllers.ShowHp(window, this, CurrentMap, result.Item4);
                            }));
                        StateChanged();
                        }
                    }
            }
        }
        private bool IndexNotOutOfRange()
        {
            if (((int)Location.Y + (int)Velocity.Y < CurrentMap.GameArea.GetLength(0) - 0.5) &&
                    ((int)Location.X + (int)Velocity.X < CurrentMap.GameArea.GetLength(1) - 2) &&
                    ((int)Location.X + (int)Velocity.X > 0) &&
                    ((int)Location.Y + (int)Velocity.Y > 0))
                return true; else return false;

            
            }
        private bool checkCollisionThrowY()
        {
            var mp = CurrentMap;
            bool reverse = !(Velocity.Y > 0);
            int sticher = Velocity.Y > 0 ? 1 : -1;
            var counter = sticher == 1 ? 1 : -1;
            var edge = Velocity.Y > 0 ? mp.GameArea.GetLength(0) : 0;
            while (counter + (int)Location.Y != edge && counter != 40 * sticher)
            {
                var startX = Location.X - 50 < 0 ? 0 : Location.X - 50;
                var endX = Location.X + 40 > mp.GameArea.GetLength(0) - 1 
                    ? mp.GameArea.GetLength(0) : Location.X + 40;
                for (int i = (int)startX; i < (int)endX; i++)
                {
                    try
                    {
                        if (mp.GameArea[(int)counter + (int)Location.Y, i] != 0 )
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
                if (reverse) { counter--; } else counter++;
            }
            return true;
        }
        private bool checkColissionThrowX()
        {
            var mp = CurrentMap;
            int switcher = Velocity.X > 0 ? 1 : -1;
            var counter = switcher == 1 ? 1 : -1;
            bool reverse = !(Velocity.X > 0);
            var edge = Velocity.X > 0 ? mp.GameArea.GetLength(0) - 1 : 0;
            while (counter != 50 * switcher && (int)Location.X + counter != edge)
            {
                var startY = Location.Y - 50 < 0 ? 0 : Location.Y - 50;
                var endY = Location.Y + 50 > mp.GameArea.GetLength(0) - 1 ? 
                    mp.GameArea.GetLength(0) - 1 : Location.Y + 50;
                for (int i = (int)startY; i < (int)endY; i++)
                {
                    try
                    {
                        if (mp.GameArea[i, (int)counter + (int)Location.X] != 0)
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
                if (reverse) { counter--; } else counter++;
            }
            return true;
        }
        private bool checkCollision(Tools.Axe axe)
        {
            if (IndexNotOutOfRange())
            {
                if (axe == Tools.Axe.Y) return checkCollisionThrowY();
                else if (axe == Tools.Axe.X) return checkColissionThrowX();
            }
            return false;
        }
        public void DoStep(Tools.Axe axis, MyFrom window)
        {
            var counter = 1;
            var lastDate = DateTime.Now;
            switch (axis)
            {
                case Tools.Axe.Y:
                    while (Velocity.Y != 0) {
                        if (checkCollision(axis))
                        {
                            var cache = new Location();
                            cache.Item1 = this.Location;
                            this.Location += SpeedMode == Mode.Slow ? new Vector(0, Velocity.Y) : 
                                new Vector(0, Velocity.Y) * 2;
                            //window.BeginInvoke(new Action(() => window.Text = CurrentDirection.ToString()));
                            cache.Item2 = this.Location;
                            CurrentMap.StateChanged(cache);
                            StateChanged();
                        }
                        if (Location.Y - 200 -  AbsoluteAmountYOffset < 0 || 
                            Location.Y - AbsoluteAmountYOffset + 200 >= 600)
                            AbsoluteAmountYOffset = Location.Y + 200 - 600;

                        Thread.Sleep(100);
                    }
                    break;
                case Tools.Axe.X:
                    while (Velocity.X != 0)
                    {
                        if ((int)(DateTime.Now - lastDate).TotalMilliseconds > 100)
                        {
                            counter++;
                            lastDate = DateTime.Now;
                            if (counter == 6) counter = 1;
                        }
                        if (checkCollision(axis))
                        {
                            var cache = new Location();
                            cache.Item1 = this.Location;
                            this.Location += SpeedMode == Mode.Slow ? new Vector(Velocity.X, 0) :
                                new Vector(Velocity.X, 0) * 3;
                            cache.Item2 = this.Location;
                            CurrentMap.StateChanged(cache);
                            Image temporary = Image.FromFile($"./textures/character{counter}.png");
                            if (Velocity.X < 0) temporary.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            CurrentTexture = temporary;
                            StateChanged();
                        }
                        if (Location.X  - 200 - AbsoluteAmountXOffset < 0 || Location.X + 200 - AbsoluteAmountXOffset >= 600)
                            AbsoluteAmountXOffset = Location.X + 200 - 600;
                        Thread.Sleep(50);
                    }
                    break;
            }
            if (Velocity.X == 0)
            {
                Image temporaryCache = Image.FromFile($"./textures/character.png");
                if (CurrentDirection == Direction.Left) temporaryCache.RotateFlip(RotateFlipType.RotateNoneFlipX);
                CurrentTexture = temporaryCache;
            }
            StateChanged();
        }
    }
}

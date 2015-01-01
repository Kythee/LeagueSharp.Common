﻿#region LICENSE

/*
 Copyright 2014 - 2014 LeagueSharp
 Orbwalking.cs is part of LeagueSharp.Common.
 
 LeagueSharp.Common is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 LeagueSharp.Common is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with LeagueSharp.Common. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#region

using System.Linq;
using SharpDX;

#endregion

namespace LeagueSharp.Common
{
    public static class Items
    {
        /// <summary>
        /// Returns true if the hero has the item.
        /// </summary>
        public static bool HasItem(string name, Obj_AI_Hero hero = null)
        {
            return (hero ?? ObjectManager.Player).InventoryItems.Any(slot => slot.Name == name);
        }

        /// <summary>
        /// Returns true if the hero has the item.
        /// </summary>
        public static bool HasItem(int id, Obj_AI_Hero hero = null)
        {
            return (hero ?? ObjectManager.Player).InventoryItems.Any(slot => slot.Id == (ItemId)id);
        }

        /// <summary>
        /// Retruns true if the player has the item and its not on cooldown.
        /// </summary>
        public static bool CanUseItem(string name)
        {
            foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Name == name))
            {
                var inst = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell =>
                    (int)spell.Slot == slot.Slot + (int)SpellSlot.Item1);
                return inst != null && inst.State == SpellState.Ready;
            }

            return false;
        }

        /// <summary>
        /// Retruns true if the player has the item and its not on cooldown.
        /// </summary>
        public static bool CanUseItem(int id)
        {
            foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id))
            {
                var inst = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell =>
                    (int)spell.Slot == slot.Slot + (int)SpellSlot.Item1);
                return inst != null && inst.State == SpellState.Ready;
            }

            return false;
        }

        /// <summary>
        /// Casts the item on the target.
        /// </summary>
        public static bool UseItem(string name, Obj_AI_Base target = null)
        {
            foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Name == name))
            {
                if (target != null)
                {
                    return ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, target);
                }
                else
                {
                    return ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot);
                }
            }

            return false;
        }

        /// <summary>
        /// Casts the item on the target.
        /// </summary>
        public static bool UseItem(int id, Obj_AI_Base target = null)
        {
            foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id))
            {
                if (target != null)
                {
                    return ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, target);
                }
                else
                {
                    return ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot);
                }
            }

            return false;
        }

        /// <summary>
        /// Casts the item on a Vector2 position.
        /// </summary>
        public static bool UseItem(int id, Vector2 position)
        {
            return UseItem(id, position.To3D());
        }

        /// <summary>
        /// Casts the item on a Vector3 position.
        /// </summary>
        public static bool UseItem(int id, Vector3 position)
        {
            if (position != Vector3.Zero)
            {
                foreach (var slot in ObjectManager.Player.InventoryItems.Where(slot => slot.Id == (ItemId)id))
                {
                    return ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, position);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the ward slot.
        /// </summary>
        public static InventorySlot GetWardSlot()
        {
            var wardIds = new[] { 3340, 3350, 3361, 3154, 2045, 2049, 2050, 2044 };
            return (from wardId in wardIds
                    where CanUseItem(wardId)
                    select ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId))
                .FirstOrDefault();
        }

        public class Item
        {
            public int Id { get; private set; }
            public float Range { get; private set; }
            public float RangeSqr { get; private set; }

            public Item(int id, float range = 0)
            {
                Id = id;
                Range = range;
                RangeSqr = range * range;
            }

            public bool IsInRange(Obj_AI_Base target)
            {
                return IsInRange(target.ServerPosition);
            }

            public bool IsInRange(Vector2 target)
            {
                return IsInRange(target.To3D());
            }

            public bool IsInRange(Vector3 target)
            {
                return ObjectManager.Player.ServerPosition.Distance(target, true) < RangeSqr;
            }

            public bool IsOwned()
            {
                return HasItem(Id);
            }

            public bool IsReady()
            {
                return CanUseItem(Id);
            }

            public bool Cast()
            {
                if (IsReady())
                {
                    UseItem(Id);
                    return true;
                }

                return false;
            }

            public bool Cast(Obj_AI_Base target)
            {
                return Cast(target.ServerPosition);
            }

            public bool Cast(Vector2 position)
            {
                return Cast(position.To3D());
            }

            public bool Cast(Vector3 position)
            {
                if (IsReady() && IsInRange(position))
                {
                    UseItem(Id, position);
                    return true;
                }

                return false;
            }

            public void Buy()
            {
                ObjectManager.Player.BuyItem((ItemId)Id);
            }
        }
    }
}
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using GentleTouch.Windows;
using System;

namespace GentleTouch.Parser {
    class CombatParser : IDisposable {
        private static CombatParser? _sharedInstance = null;
        public static CombatParser getInstance() { 
            if(_sharedInstance == null)
                _sharedInstance = new CombatParser();
            return _sharedInstance;
        }

        public void init() {
            Service.FlyTextGui.FlyTextCreated += this.FlyTextCreated;
        }

        public void FlyTextCreated(
            ref FlyTextKind kind,
            ref int val1,
            ref int val2,
            ref SeString text1,
            ref SeString text2,
            ref uint color,
            ref uint icon,
            ref uint damageTypeIcon,
            ref float yOffset,
            ref bool handled) {
            switch (kind) {
                default: return; // Nothing we care about
                // Crit:
                case FlyTextKind.AutoAttackOrDotCrit:
                case FlyTextKind.DamageCrit:
                case FlyTextKind.CriticalHit4:
                    Connection.getInstance().Vibrate(1300,.5f);
                    break;
                // Healer Crit:
                case FlyTextKind.HealingCrit:
                    Connection.getInstance().Vibrate(1300,.7f);
                    break;
                // DH:
                case FlyTextKind.DamageDh:
                case FlyTextKind.AutoAttackOrDotDh:
                    Connection.getInstance().Vibrate(1300,.25f);
                    break;
                // Crit DH:
                case FlyTextKind.AutoAttackOrDotCritDh:
                    Connection.getInstance().Vibrate(1300, .6f);
                    break;
            }
        }

        public static void Close() {
            _sharedInstance?.Dispose();
            _sharedInstance = null;
        }

        public void Dispose() {
            Service.FlyTextGui.FlyTextCreated -= FlyTextCreated;
            GC.SuppressFinalize(this);
        }
    }
}

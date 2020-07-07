using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TimsWpfControls.MahAppsHelper
{
    public static class TreeHelper
    {
        public static bool IsDescendantOf(this DependencyObject node, DependencyObject reference)
        {
            bool success = false;

            DependencyObject curr = node;

            while (curr != null)
            {
                if (curr == reference)
                {
                    success = true;
                    break;
                }

                if (curr is Popup popup)
                {
                    curr = popup;

                    if (popup != null)
                    {
                        // Try the poup Parent
                        curr = popup.Parent;

                        // Otherwise fall back to placement target
                        if (curr == null)
                        {
                            curr = popup.PlacementTarget;
                        }
                    }
                }
                else // Otherwise walk tree
                {
                    curr = curr.GetParentObject();
                }

            }

            return success;
        }
    }
}

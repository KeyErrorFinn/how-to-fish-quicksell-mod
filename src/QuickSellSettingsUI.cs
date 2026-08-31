using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KeyErrorFinn.QuickSell
{
    internal sealed class QuickSellSettingsUI : IDisposable
    {
        private readonly ManualLogSource _log;
        private readonly List<GameObject> _nativeSettingsRows = new List<GameObject>();

        public QuickSellSettingsUI(ManualLogSource log)
        {
            _log = log;
        }

        public void Install(ConfigEntry<bool> sellFloorItems, ConfigEntry<bool> sellInventoryItems)
        {
            var bloodLabel = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()
                .FirstOrDefault(text => text != null && !string.IsNullOrEmpty(text.text) && text.text.Trim() == "Blood");
            if (bloodLabel == null || bloodLabel.transform.parent == null || bloodLabel.transform.parent.parent == null)
            {
                _log.LogWarning("Gameplay options are not loaded yet; QuickSell settings will appear after the next ScriptEngine reload.");
                return;
            }

            var templateRow = bloodLabel.transform.parent;
            var layout = templateRow.parent;
            AddSpacer(layout, "MassSellSettingsSpacer", 6f);
            AddToggle(layout, templateRow, "QuickSell: Floor Fish", sellFloorItems);
            AddToggle(layout, templateRow, "QuickSell: Inventory Fish/Creatures", sellInventoryItems);
            AddSpacer(layout, "MassSellSettingsBottomPadding", 10f);
            MakeScrollable(layout);
        }

        public void Dispose()
        {
            foreach (var row in _nativeSettingsRows)
            {
                if (row != null)
                    UnityEngine.Object.Destroy(row);
            }
            _nativeSettingsRows.Clear();
        }

        private void AddSpacer(Transform layout, string name, float height)
        {
            var spacer = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            spacer.transform.SetParent(layout, false);
            var layoutElement = spacer.GetComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;
            layoutElement.flexibleHeight = 0f;
            _nativeSettingsRows.Add(spacer);
        }

        private void AddToggle(Transform layout, Transform templateRow, string labelText, ConfigEntry<bool> setting)
        {
            var row = UnityEngine.Object.Instantiate(templateRow.gameObject, layout);
            row.name = "MassSell" + labelText.Replace(" ", string.Empty) + "Row";
            _nativeSettingsRows.Add(row);

            var label = row.GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault();
            if (label != null)
            {
                foreach (var behaviour in label.GetComponents<Behaviour>())
                {
                    if (behaviour.GetType().Name == "LocalizeStringEvent")
                        behaviour.enabled = false;
                }
                label.text = labelText;
            }

            var toggle = row.GetComponentInChildren<Toggle>(true);
            if (toggle == null)
            {
                _log.LogError("Native settings toggle template was missing its Toggle component.");
                UnityEngine.Object.Destroy(row);
                return;
            }

            toggle.onValueChanged = new Toggle.ToggleEvent();
            toggle.isOn = setting.Value;
            toggle.onValueChanged.AddListener(value => setting.Value = value);
        }

        private static void MakeScrollable(Transform layout)
        {
            Transform display;
            GameObject viewport;
            if (layout.parent != null && layout.parent.name == "MassSellGameplayViewport")
            {
                viewport = layout.parent.gameObject;
                display = viewport.transform.parent;
            }
            else
            {
                display = layout.parent;
                if (display == null)
                    return;

                viewport = new GameObject("MassSellGameplayViewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
                viewport.transform.SetParent(display, false);
                layout.SetParent(viewport.transform, false);
            }

            if (display == null)
                return;

            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = new Vector2(0f, 8f);
            viewportRect.offsetMax = new Vector2(-18f, -8f);
            var viewportImage = viewport.GetComponent<Image>() ?? viewport.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0f);
            viewportImage.raycastTarget = true;

            var contentRect = (RectTransform)layout;
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0f, contentRect.sizeDelta.y);

            var fitter = layout.GetComponent<ContentSizeFitter>() ?? layout.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var oldScrollRect = display.GetComponent<ScrollRect>();
            if (oldScrollRect != null)
                UnityEngine.Object.Destroy(oldScrollRect);

            var scrollRect = viewport.GetComponent<ScrollRect>() ?? viewport.AddComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 35f;
            scrollRect.verticalScrollbar = CreateOrUpdateScrollbar(display);
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            scrollRect.verticalScrollbarSpacing = 4f;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }

        private static Scrollbar CreateOrUpdateScrollbar(Transform display)
        {
            var existing = display.Find("MassSellGameplayScrollbar");
            var scrollbarObject = existing != null
                ? existing.gameObject
                : new GameObject("MassSellGameplayScrollbar", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
            if (existing == null)
                scrollbarObject.transform.SetParent(display, false);

            var scrollbarRect = scrollbarObject.GetComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1f, 0f);
            scrollbarRect.anchorMax = new Vector2(1f, 1f);
            scrollbarRect.pivot = new Vector2(1f, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(10f, -16f);
            scrollbarRect.anchoredPosition = new Vector2(-5f, 0f);

            var track = scrollbarObject.GetComponent<Image>();
            track.color = new Color(0f, 0f, 0f, 0.32f);
            track.raycastTarget = true;

            var handle = scrollbarObject.transform.Find("Sliding Area/Handle") as RectTransform;
            if (handle == null)
            {
                var slidingArea = new GameObject("Sliding Area", typeof(RectTransform));
                slidingArea.transform.SetParent(scrollbarObject.transform, false);
                var slidingAreaRect = slidingArea.GetComponent<RectTransform>();
                slidingAreaRect.anchorMin = Vector2.zero;
                slidingAreaRect.anchorMax = Vector2.one;
                slidingAreaRect.offsetMin = new Vector2(2f, 2f);
                slidingAreaRect.offsetMax = new Vector2(-2f, -2f);

                var handleObject = new GameObject("Handle", typeof(RectTransform), typeof(Image));
                handleObject.transform.SetParent(slidingArea.transform, false);
                handle = handleObject.GetComponent<RectTransform>();
                handle.anchorMin = Vector2.zero;
                handle.anchorMax = Vector2.one;
                handle.offsetMin = Vector2.zero;
                handle.offsetMax = Vector2.zero;
            }

            var handleImage = handle.GetComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.82f);
            handleImage.raycastTarget = true;

            var scrollbar = scrollbarObject.GetComponent<Scrollbar>();
            scrollbar.handleRect = handle;
            scrollbar.targetGraphic = handleImage;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            return scrollbar;
        }
    }
}

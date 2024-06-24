using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SRPConverter
{
    /// <summary>
    /// Additional window to show progress and log
    /// </summary>
    public class ProgressWindow : EditorWindow
    {
        public float Progress01
        {
            get => _bar.value;
            set => _bar.value = Mathf.Clamp01(value);
        }
        private ProgressBar _bar;
        private ScrollView _view;
        private static DateTime _createdTime;

        /// <summary>
        /// CreateWindow and reset progress data if exist
        /// </summary>
        /// <param name="header">window header</param>
        /// <returns>new/reseted progress window</returns>
        public static ProgressWindow ShowWindow(string header)
        {
            var window = GetWindow<ProgressWindow>();
            window.titleContent = new GUIContent(header);

            if (_createdTime != DateTime.Now)
            {
                while (window.rootVisualElement.childCount > 0)
                    window.rootVisualElement.RemoveAt(0);
                window.CreateInternal();
            }

            return window;
        }

        private void CreateGUI()
        {
            _createdTime = DateTime.Now;
            CreateInternal();
        }

        private void CreateInternal()
        {
            var menu = new VisualElement();
            rootVisualElement.Add(menu);

            _bar = new ProgressBar();
            _bar.lowValue = 0;
            _bar.highValue = 1;
            menu.Add(_bar);

            _view = new ScrollView(ScrollViewMode.Vertical);
            rootVisualElement.Add(_view);
        }



        public void Log(string log)
        {
            var text = new Label(log);
            _view.Add(text);
        }

        public void LogWarning(string log)
        {
            var text = new Label(log);
            text.style.color = Color.yellow;
            _view.Add(text);
        }

        public void LogError(string log)
        {
            var text = new Label(log);
            text.style.color = Color.red;
            _view.Add(text);
        }

        /// <summary>
        /// Complete progress and add Done button
        /// </summary>
        /// <param name="log">last log</param>
        public void Done(string log = default)
        {
            if (log != default)
            {
                var text = new Label(log);
                text.style.color = Color.green;
                _view.Add(text);
            }

            _bar.value = 1;

            var done = new Button(() => Close());
            done.text = "Done";
            done.style.color = Color.green;
            _bar.parent.Add(done);
        }
    }
}
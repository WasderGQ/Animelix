using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GeneralConstructions.ConstructiveAssistants
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Screen Scaler")]
    [CustomEditor(typeof(ProportionerByScreen))] // Bu satırı ekleyin
    [RequireComponent(typeof(CanvasScaler), typeof(Canvas))]
    public class ProportionerByScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;
    
        [SerializeField] private float _widthMin;
        [SerializeField] private float _widthMinRate;
        [SerializeField] private float _heightMin;
        [SerializeField] private float _heightMinRate;
        [SerializeField] private float _aspectRatioMin;
        [SerializeField] private float _aspectRatioMinRate;
        [SerializeField] private List<RectTransform> _widthRateTransforms;
        [SerializeField] private List<RectTransform> _heightRateTransforms;
        [SerializeField] private List<RectTransform> _aspectRatioRateTransforms;

        private void Start()
        {
            FixRatioByWidth();
            FixRationByHeight();
            FixRationByAspectRatio();
        
        }
    

        private void FixRatioByWidth()
        {
            float scaleRate = 1;
            if (_canvas.sizeDelta.x < _canvasScaler.referenceResolution.x)
            {
                float range  =_canvasScaler.referenceResolution.x - _widthMin;
                float unitProportioningRate = (_widthMinRate / range);
                scaleRate = ((_canvas.sizeDelta.x - _widthMin) * unitProportioningRate ) + _widthMinRate;
            
            }
            else if (_canvas.sizeDelta.x < _widthMin)
            {
                scaleRate = _widthMinRate;
            }
        
            if (_widthRateTransforms.Count > 0)
            {
                foreach (var rectTransform in _widthRateTransforms)
                {
                    rectTransform.localScale = new Vector3(scaleRate, scaleRate, scaleRate);
                }
            }
            else
            {
                Debug.Log("Width Rate Transform List is empty");
            }
        }
        private void FixRationByHeight()
        {
            float range  =_canvasScaler.referenceResolution.y - _heightMin;
            float unitProportioningRate = (_heightMinRate / range);
            float scaleRate = ((_canvas.sizeDelta.y - _heightMin) * unitProportioningRate) + _heightMinRate;
            if (_heightRateTransforms.Count != 0)
            {
                foreach (var rectTransform in _heightRateTransforms)
                {
                    rectTransform.localScale = new Vector3(scaleRate, scaleRate, scaleRate);
                }
            }
            else
            {
                Debug.Log("Height Rate Transform List is empty");
            }
        }
        private void FixRationByAspectRatio()
        {
            float range  =_canvasScaler.referenceResolution.x / _canvasScaler.referenceResolution.y - _aspectRatioMin;
            float unitProportioningRate = (_aspectRatioMinRate / range);
            float scaleRate = ((_canvas.sizeDelta.x / _canvas.sizeDelta.y - _aspectRatioMin) * unitProportioningRate) + _aspectRatioMinRate;
            if (_aspectRatioRateTransforms.Count != 0)
            {
                foreach (var rectTransform in _aspectRatioRateTransforms)
                {
                    rectTransform.localScale = new Vector3(scaleRate, scaleRate, scaleRate);
                }
            }
            else
            {
                Debug.Log("Aspect Ratio Rate Transform List is empty");
            }
        }
    }




    [CustomEditor(typeof(ProportionerByScreen))]
    public class ScreenScalerEditor : Editor
    {
        private Texture2D icon;

        private void OnEnable()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Images/OnDevEditor/PNG/Scale.png");
            if(icon == null)
                Debug.Log("Icon is null");
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return icon;
        }
    }
}
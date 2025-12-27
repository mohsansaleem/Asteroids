using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Oryx
{
    public class SimpleStateComponent : MonoBehaviour
    {

        private bool _isFilled;

        [SerializeField] public List<SimpleStateModel> SimpleStateModels;
        [SerializeField] public int Version = 1;

        private Dictionary<string, SimpleStateModel> _cacheData;
        private Dictionary<string, List<string>> _cacheAnims;
        private List<Tweener> _cachedTweens;
        private List<string> _cacheIndexOrder;
        private Animation _animation;

        private Dictionary<int, string> _layerStatus;

        [NonSerialized] public bool EnableDebugLogs = false;
        [NonSerialized] public string DebugLogFilter = "";

        public static Dictionary<ActionType, Type> EnumToType = new Dictionary<ActionType, Type>()
        {
            { ActionType.NoAction, typeof(NoActionModel) },
            { ActionType.PlayAnimation, typeof(AnimationActionModel) },
            { ActionType.SetSprite, typeof(SetSpriteActionModel) },
            { ActionType.UpdateCanvasGroup, typeof(UpdateCanvasGroupActionModel) },
            { ActionType.UpdateMeshRenderer, typeof(MeshRendererActionModel) },
            { ActionType.UpdateCollider, typeof(ColliderActionModel) },
            { ActionType.SwapMaterial, typeof(SwapMaterialActionModel) },
            { ActionType.SetActive, typeof(SetActiveActionModel) },
            { ActionType.SetTransform, typeof(SetTransformActionModel) },
            { ActionType.AnimateColor, typeof(AnimateColorActionModel) },
            { ActionType.SetTransformProperty, typeof(SetPropertyActionModel) },
            { ActionType.SetGameCamera, typeof(SetGameCameraActionModel) },
            { ActionType.UpdateMultiCanvasGroup, typeof(UpdateMultiCanvasGroupActionModel) },
            { ActionType.StopAnimation, typeof(StopAnimationActionModel) },
            { ActionType.SetText, typeof(SetTextActionModel) },
            { ActionType.SetTextColor, typeof(SetTextColorActionModel) },
            { ActionType.SetImageColor, typeof(SetImageColorActionModel) },
            { ActionType.SetGraphicsRaycasters, typeof(SetGraphicsRaycasterActionModel) },
            { ActionType.SetRectProperty, typeof(SetRectPropertyActionModel) },
            { ActionType.SwapMaterialList, typeof(SwapMaterialListActionModel) },
            { ActionType.AnimateSharedColor, typeof(AnimateSharedColorActionModel) },
            { ActionType.UpdateCanvasGroupInteraction, typeof(UpdateCanvasGroupInteractionActionModel) },
            { ActionType.SetTransformByName, typeof(SetTransformByNameActionModel) },
            { ActionType.SetRectTransformByName, typeof(SetRectTransformByNameActionModel) },
            { ActionType.PlayExternalState, typeof(PlayExternalStateModel) },
            { ActionType.SetActiveList, typeof(SetActiveListActionModel) },
            { ActionType.PlaySoundEvent, typeof(PlaySoundEventActionModel) },
            { ActionType.SetComponentEnabled, typeof(SetComponentEnabledActionModel) },
            { ActionType.SetRectTransform, typeof(SetRectTransformActionModel) },
            { ActionType.PlayAnimator, typeof(PlayAnimatorActionModel) },
            { ActionType.SetColliderActive, typeof(SetColliderActiveActionModel) },
            { ActionType.DoRotate, typeof(DoRotateActionModel) },
            { ActionType.DoShakePosition, typeof(DoShakePositionActionModel) },
            { ActionType.DoPunchScale, typeof(DoPunchScaleActionModel) },
            { ActionType.DoMove, typeof(DoMoveActionModel) },
            { ActionType.DoFade, typeof(DoFadeActionModel) },
            { ActionType.SetMaterial, typeof(SetMaterialActionModel) },
            { ActionType.DoFadeGroup, typeof(DoFadeGroupActionModel) },
            { ActionType.PlayAnimatorSafe, typeof(PlayAnimatorSafeActionModel) },
            {ActionType.StopSound, typeof(StopSoundActionModel)}

        };

        public static Dictionary<Type, ActionType> TypeToEnum = new Dictionary<Type, ActionType>()
        {
            { typeof(NoActionModel), ActionType.NoAction },
            { typeof(AnimationActionModel), ActionType.PlayAnimation },
            { typeof(SetSpriteActionModel), ActionType.SetSprite },
            { typeof(UpdateCanvasGroupActionModel), ActionType.UpdateCanvasGroup },
            { typeof(MeshRendererActionModel), ActionType.UpdateMeshRenderer },
            { typeof(ColliderActionModel), ActionType.UpdateCollider },
            { typeof(SwapMaterialActionModel), ActionType.SwapMaterial },
            { typeof(SetActiveActionModel), ActionType.SetActive },
            { typeof(SetTransformActionModel), ActionType.SetTransform },
            { typeof(AnimateColorActionModel), ActionType.AnimateColor },
            { typeof(SetPropertyActionModel), ActionType.SetTransformProperty },
            { typeof(SetGameCameraActionModel), ActionType.SetGameCamera },
            { typeof(UpdateMultiCanvasGroupActionModel), ActionType.UpdateMultiCanvasGroup },
            { typeof(StopAnimationActionModel), ActionType.StopAnimation },
            { typeof(SetTextActionModel), ActionType.SetText },
            { typeof(SetTextColorActionModel), ActionType.SetTextColor },
            { typeof(SetImageColorActionModel), ActionType.SetImageColor },
            { typeof(SetGraphicsRaycasterActionModel), ActionType.SetGraphicsRaycasters },
            { typeof(SetRectPropertyActionModel), ActionType.SetRectProperty },
            { typeof(SwapMaterialListActionModel), ActionType.SwapMaterialList },
            { typeof(AnimateSharedColorActionModel), ActionType.AnimateSharedColor },
            { typeof(UpdateCanvasGroupInteractionActionModel), ActionType.UpdateCanvasGroupInteraction },
            { typeof(SetTransformByNameActionModel), ActionType.SetTransformByName },
            { typeof(SetRectTransformByNameActionModel), ActionType.SetRectTransformByName },
            { typeof(PlayExternalStateModel), ActionType.PlayExternalState },
            { typeof(SetActiveListActionModel), ActionType.SetActiveList },
            { typeof(PlaySoundEventActionModel), ActionType.PlaySoundEvent },
            { typeof(SetComponentEnabledActionModel), ActionType.SetComponentEnabled },
            { typeof(SetRectTransformActionModel), ActionType.SetRectTransform },
            { typeof(PlayAnimatorActionModel), ActionType.PlayAnimator },
            { typeof(SetColliderActiveActionModel), ActionType.SetColliderActive },
            { typeof(DoRotateActionModel), ActionType.DoRotate },
            { typeof(DoShakePositionActionModel), ActionType.DoShakePosition },
            { typeof(DoPunchScaleActionModel), ActionType.DoPunchScale },
            { typeof(DoMoveActionModel), ActionType.DoMove },
            { typeof(DoFadeActionModel), ActionType.DoFade },
            { typeof(SetMaterialActionModel), ActionType.SetMaterial },
            { typeof(DoFadeGroupActionModel), ActionType.DoFadeGroup },
            { typeof(PlayAnimatorSafeActionModel), ActionType.PlayAnimatorSafe },
            { typeof(StopSoundActionModel), ActionType.StopSound },
        };

        private void Awake()
        {
            if (Version != 1)
            {

            }

            Fill();
        }

        private void Reset()
        {
            Version = 1;
        }


        private string _currentState = "";
        //private string _previousState = "";

        public string CurrentState(int layer = -1)
        {
            return _currentState;
        }

        void Start()
        {
            Fill();
        }


        private void Fill()
        {
            if (_isFilled)
                return;


            if (SimpleStateModels == null)
                SimpleStateModels = new List<SimpleStateModel>();
            _animation = this.GetComponent<Animation>();
            _cacheIndexOrder = new List<string>();
            _cachedTweens = new List<Tweener>();
            if (_animation != null)
            {
#if UNITY_EDITOR
                if (_animation.playAutomatically)
                {
                    Debug.LogError(string.Format("Play automatically is set in the animation in game object :{0}, which is used by FSimpleState , this will cause issues",this.gameObject.name));
                }
#endif
                _animation.playAutomatically = false;
            }

            _cacheData = new Dictionary<string, SimpleStateModel>();
            _cacheAnims = new Dictionary<string, List<string>>();
            _layerStatus = new Dictionary<int, string>();


            FillNew();

            if (_cacheAnims.Count <= 0 && _animation != null)
            {
                _animation.enabled = false;
            }

            _isFilled = true;
        }

        private void FillNew()
        {
            foreach (SimpleStateModel stateModel in SimpleStateModels)
            {
                if (stateModel.BasicActionModels == null || stateModel.BasicActionModels.Count <= 0)
                    continue;
                _cacheData[stateModel.State] = stateModel;
                if (stateModel.CustomDuration > 0)
                {
                    stateModel.Duration = stateModel.CustomDuration;
                }
                else
                {
                    foreach (IBasicActionModel actionModel in stateModel.BasicActionModels)
                    {
                        switch (actionModel)
                        {
                            case AnimationActionModel animationActionModel:
                                if (_animation == null)
                                    continue;
                                AnimationClip animationClip = _animation.GetClip(animationActionModel.Clip);
                                if (animationClip == null)
                                {
                                    //Throw error
                                    continue;
                                }

                                animationActionModel.Length = animationClip.length;
                                if (animationActionModel.Length > stateModel.Duration)
                                {
                                    stateModel.Duration = animationActionModel.Length;
                                }

                                if (!_cacheAnims.ContainsKey(stateModel.State))
                                {
                                    _cacheAnims[stateModel.State] = new List<string>();
                                }

                                _cacheAnims[stateModel.State].Add(animationActionModel.Clip);
                                break;
                        }
                    }
                }

            }
        }

        public SimpleStateModel GetSimpleStateModel(string stateName)
        {
            if (_cacheData == null)
            {
                Fill();
            }

            if (_cacheData == null)
                return null;
            _cacheData.TryGetValue(stateName, out var result);
            return result;
        }

        public void PlayState(string stateName, bool checkIsActiveInHierarchy = false, bool forcePlay = false,
            bool updateCurrent = true)
        {
            try
            {
                if (_cacheData == null)
                {
                    Fill();
                }

                if (_cacheData == null)
                {
                    return;
                }

                if (checkIsActiveInHierarchy && !gameObject.activeInHierarchy)
                {

                    return;
                }

                if (!_cacheData.ContainsKey(stateName))
                {
                    return;
                }

                if (this._currentState == stateName && !forcePlay)
                {
                    return;
                }

                if (updateCurrent)
                {
                    this._currentState = stateName;
                }

                SimpleStateModel simpleStateModel = _cacheData[stateName];

                if (simpleStateModel != null && _layerStatus.ContainsKey(simpleStateModel.LayerIndex))
                {
                    string previousState = _layerStatus[simpleStateModel.LayerIndex];
                    if (_cacheAnims.ContainsKey(previousState))
                    {
                        List<string> previousAnims = _cacheAnims[previousState];
                        foreach (string previousClip in previousAnims)
                        {
                            AnimationState animationState = _animation[previousClip];
                            if (animationState != null && animationState.enabled)
                            {
                                animationState.enabled = false;
                            }
                        }
                    }

                    _layerStatus.Remove(simpleStateModel.LayerIndex);
                }

                if (simpleStateModel != null && simpleStateModel.BasicActionModels != null)
                {
                    foreach (IBasicActionModel actionModel in simpleStateModel.BasicActionModels)
                    {

                        ExecuteEachStateActionNew(actionModel, stateName);
                    }
                }


            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void PlayStateAsSingle(string stateName)
        {
            PlayState(stateName, false, true);
        }

        public void PlayActions(string stateName)
        {
            PlayState(stateName, false, true, false);
        }


        public void ExecuteEachStateActionNew(IBasicActionModel actionModel, string stateName)
        {
            switch (actionModel)
            {
                case AnimationActionModel animationActionModel:
                    ExecutePlayAnimation(animationActionModel);
                    break;
                case SetSpriteActionModel setSpriteActionModel:
                    ExecuteSetSpriteAction(setSpriteActionModel);
                    break;
                case UpdateCanvasGroupActionModel updateCanvasGroupActionModel:
                    ExecuteUpdateCanvasGroup(updateCanvasGroupActionModel);
                    break;
                case MeshRendererActionModel meshRendererActionModel:
                    ExecuteUpdateMeshRenderer(meshRendererActionModel);
                    break;
                case ColliderActionModel colliderActionModel:
                    ExecuteUpdateCollider(colliderActionModel);
                    break;
                case SwapMaterialActionModel swapMaterialActionModel:
                    ExecuteSwapMaterial(swapMaterialActionModel);
                    break;
                case SwapMaterialListActionModel swapMaterialListActionModel:
                    ExecuteSwapMaterialList(swapMaterialListActionModel);
                    break;
                case SetActiveActionModel setActiveActionModel:
                    ExecuteSetActive(setActiveActionModel);
                    break;
                case SetActiveListActionModel setActiveListActionModel:
                    ExecuteSetActiveList(setActiveListActionModel);
                    break;
                case SetTransformActionModel setTransformActionModel:
                    ExecuteSetTransformAction(setTransformActionModel);
                    break;
                case AnimateColorActionModel animateColorActionModel:
                    ExecuteAnimateColor(animateColorActionModel);
                    break;
                case AnimateSharedColorActionModel animateSharedColorActionModel:
                    ExecuteSharedAnimateColor(animateSharedColorActionModel);
                    break;
                case SetPropertyActionModel setPropertyActionModel:
                    ExecuteSetPropertyAction(setPropertyActionModel);
                    break;
                case UpdateMultiCanvasGroupActionModel updateMultiCanvasGroupActionModel:
                    ExecuteUpdateMultiCanvasGroup(updateMultiCanvasGroupActionModel);
                    break;
                case UpdateCanvasGroupInteractionActionModel updateCanvasGroupInteractionActionModel:
                    ExecuteCanvasGroupInteraction(updateCanvasGroupInteractionActionModel);
                    break;
                case StopAnimationActionModel stopAnimationActionModel:
                    ExecuteStopAnimation(stopAnimationActionModel);
                    break;
                //UI
                case SetTextActionModel setTextActionModel:
                    ExecuteSetText(setTextActionModel);
                    break;
                case SetTextColorActionModel setTextColorActionModel:
                    ExecuteSetTextColor(setTextColorActionModel);
                    break;
                case SetImageColorActionModel setImageColorActionModel:
                    ExecuteSetImageColor(setImageColorActionModel);
                    break;
                case SetGraphicsRaycasterActionModel setGraphicsRaycasterActionModel:
                    ExecuteSetGraphicsRaycaster(setGraphicsRaycasterActionModel);
                    break;
                case SetRectPropertyActionModel setRectPropertyActionModel:
                    ExecuteSetRectPropertyAction(setRectPropertyActionModel);
                    break;
                case SetRectTransformActionModel setRectTransformActionModel:
                    ExecuteSetRectTransformAction(setRectTransformActionModel);
                    break;
                case PlayExternalStateModel playExternalStateModel:
                    ExecutePlayExternalState(playExternalStateModel, stateName);
                    break;
                case SetComponentEnabledActionModel setComponentEnabledActionModel:
                    ExecuteSetComponentEnabled(setComponentEnabledActionModel);
                    break;
                case PlayAnimatorSafeActionModel playAnimatorSafeActionModel:
                    ExecutePlayAnimatorSafe(playAnimatorSafeActionModel);
                    break;
                case SetColliderActiveActionModel setColliderActiveActionModel:
                    ExecuteSetColliderActive(setColliderActiveActionModel);
                    break;
                case DoRotateActionModel doRotateActionModel:
                    ExecuteDoRotate(doRotateActionModel);
                    break;
                case DoShakePositionActionModel doShakePositionActionModel:
                    ExecuteDoShakePosition(doShakePositionActionModel);
                    break;
                case DoPunchScaleActionModel doPunchScaleActionModel:
                    ExecuteDoPunchScale(doPunchScaleActionModel);
                    break;
                case DoMoveActionModel doMoveActionModel:
                    ExecuteDoMove(doMoveActionModel);
                    break;
                case DoFadeActionModel doFadeActionModel:
                    ExecuteDoFade(doFadeActionModel);
                    break;
                case SetMaterialActionModel setMaterialActionModel:
                    ExecuteSetMaterial(setMaterialActionModel);
                    break;
                case DoFadeGroupActionModel doFadeGroupActionModel:
                    ExecuteDoFadeGroup(doFadeGroupActionModel);
                    break;
                case PlaySoundEventActionModel playSoundEventActionModel:
                    ExecutePlaySoundEvent(playSoundEventActionModel);
                    break;
                case StopSoundActionModel stopSoundActionModel:
                    ExecuteStopSoundEvent(stopSoundActionModel);
                    break;

            }
        }


        private void ExecuteSwapMaterial(SwapMaterialActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.Renderer == null || actionModel.Material == null)
                return;

            Material[] materials = new Material[actionModel.Renderer.materials.Length];

            for (int i = 0; i < materials.Length; i++) // Index:
            {
                if (i == actionModel.Index)
                {
                    materials[i] = actionModel.Material;
                }
                else
                {
                    materials[i] = actionModel.Renderer.materials[i];
                }
            }


            actionModel.Renderer.materials = materials;


            materials = null;
        }

        private void ExecuteSwapMaterialList(SwapMaterialListActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.Renderers == null || actionModel.Material == null)
                return;
            foreach (Renderer rndr in actionModel.Renderers)
            {
                if (rndr == null)
                {
                    continue;
                }

                Material[] materials = new Material[rndr.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    if (i == actionModel.Index)
                    {
                        Destroy(rndr.materials[i]);
                        materials[i] = actionModel.Material;
                    }
                    else
                    {
                        materials[i] = rndr.materials[i];
                    }

                }

                rndr.materials = materials;

                materials = null;
            }

        }

        private static void ExecuteUpdateCollider(ColliderActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.Collider == null)
                return;
            actionModel.Collider.enabled =
                actionModel.Enabled;
        }

        private static void ExecuteUpdateMeshRenderer(MeshRendererActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.MeshRenderer == null)
                return;
            actionModel.MeshRenderer.enabled =
                actionModel.Enabled;
        }

        private static void ExecuteUpdateCanvasGroup(UpdateCanvasGroupActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.CanvasGroup == null)
                return;
            actionModel.CanvasGroup.alpha =
                actionModel.Alpha;
            actionModel.CanvasGroup.interactable =
                actionModel.IsInteractible;
            actionModel.CanvasGroup.blocksRaycasts =
                actionModel.BlockRaycasts;
        }

        private static void ExecuteSetSpriteAction(SetSpriteActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.Image == null)
                return;
            actionModel.Image.sprite = actionModel.Sprite;
        }

        private void ExecutePlayAnimation(AnimationActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.Clip == null)
                return;
            if (_animation == null)
                return;

            AnimationState animationState = _animation[actionModel.Clip];
            if (animationState == null)
                return;
            animationState.layer = actionModel.Layer;
            animationState.weight = actionModel.Weight;

            if (_animation.IsPlaying(actionModel.Clip))
            {
                _animation.Stop(actionModel.Clip);
            }

            _animation.Play(actionModel.Clip);
        }

        public void AnimationSample()
        {
            _animation.Sample();
        }


        private void ExecuteSetTransformAction(SetTransformActionModel actionModel)
        {
            GameObject target = actionModel.Target;
            if (target == null)
            {
#if ART_ERRORS_TO_WARNING
#else

#endif
                return;
            }

            if (!actionModel.IsLocal)
            {
                target.transform.SetPositionAndRotation(actionModel.Position,
                    Quaternion.Euler(actionModel.Rotation));
                target.transform.localScale = actionModel.Scale;
            }
            else
            {
                target.transform.localPosition = (actionModel.Position);
                target.transform.localRotation = (Quaternion.Euler(actionModel.Rotation));
                target.transform.localScale = actionModel.Scale;
            }
        }

        private void ExecuteSetPropertyAction(SetPropertyActionModel actionModel)
        {
            List<Transform> targets = actionModel.Targets;
            if (targets == null)
            {

                return;
            }

            foreach (Transform target in targets)
            {
                Vector3 vector = Vector3.zero;
                switch (actionModel.TransformType)
                {
                    case PropertyType.PositionX:
                        vector = target.position;
                        vector.x = actionModel.Value;
                        target.position = vector;
                        break;
                    case PropertyType.PositionY:
                        vector = target.position;
                        vector.y = actionModel.Value;
                        target.position = vector;
                        break;
                    case PropertyType.PositionZ:
                        vector = target.position;
                        vector.z = actionModel.Value;
                        target.position = vector;
                        break;
                    case PropertyType.RotationX:
                        vector = target.rotation.eulerAngles;
                        vector.x = actionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationY:
                        vector = target.rotation.eulerAngles;
                        vector.y = actionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationZ:
                        vector = target.rotation.eulerAngles;
                        vector.z = actionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.ScaleX:
                        vector = target.localScale;
                        vector.x = actionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleY:
                        vector = target.localScale;
                        vector.y = actionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleZ:
                        vector = target.localScale;
                        vector.z = actionModel.Value;
                        target.localScale = vector;
                        break;
                }
            }

        }

        private void ExecuteSetRectPropertyAction(SetRectPropertyActionModel actionModel)
        {
            List<RectTransform> targets = actionModel.Targets;
            if (targets == null)
            {

                return;
            }

            foreach (RectTransform target in targets)
            {
                Vector3 vector = Vector3.zero;
                switch (actionModel.TransformType)
                {
                    case PropertyType.PositionX:
                        vector = target.anchoredPosition;
                        vector.x = actionModel.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.PositionY:
                        vector = target.anchoredPosition;
                        vector.y = actionModel.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.PositionZ:
                        vector = target.anchoredPosition;
                        vector.z = actionModel.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.RotationX:
                        vector = target.rotation.eulerAngles;
                        vector.x = actionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationY:
                        vector = target.rotation.eulerAngles;
                        vector.y = actionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationZ:
                        vector = target.rotation.eulerAngles;
                        vector.z = actionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.ScaleX:
                        vector = target.localScale;
                        vector.x = actionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleY:
                        vector = target.localScale;
                        vector.y = actionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleZ:
                        vector = target.localScale;
                        vector.z = actionModel.Value;
                        target.localScale = vector;
                        break;
                }
            }

        }

        private void ExecuteSetActive(SetActiveActionModel actionModel)
        {
            GameObject target = actionModel.Target;
            if (target == null)
            {
                return;
            }

            target.SetActive(actionModel.State);
        }

        private void ExecuteSetActiveList(SetActiveListActionModel actionModel)
        {
            if (actionModel.Targets == null ||
                actionModel.Targets.Count <= 0)
                return;
            foreach (GameObject go in actionModel.Targets)
            {
                if (go == null)
                {
                    return;
                }

                go.SetActive(actionModel.State);
            }
        }

        private void ExecutePlayExternalState(PlayExternalStateModel actionModel, string stateName)
        {
            SimpleStateComponent targetComponent = actionModel.Target;
            if (targetComponent == this && actionModel.State == stateName)
            {
                return;
            }

            if (targetComponent != null)
            {
                targetComponent.PlayState(actionModel.State, false, true);
            }
        }

        private void ExecuteAnimateColor(AnimateColorActionModel actionModel)
        {
            List<Renderer> target = actionModel.Target;
            Color targetColor = actionModel.TargetColor;
            float duration = actionModel.Duration;
            int id = actionModel.MaterialId;
            foreach (Renderer rndr in target)
            {
                if (rndr == null)
                    continue;

                try
                {
                    var rndrMaterial = rndr.materials[id];
                    rndrMaterial.DOKill();
                    if (duration <= 0F)
                    {
                        //instant interpolate
                        rndrMaterial.SetColor("_BaseColor", targetColor);
                    }
                    else
                    {
                        rndrMaterial.DOColor(targetColor, "_BaseColor", duration);
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogError(e.Message);
                }
            }

        }

        private void ExecuteSharedAnimateColor(AnimateSharedColorActionModel actionModel)
        {
            List<Renderer> target = actionModel.Target;
            Color targetColor = actionModel.TargetColor;
            float duration = actionModel.Duration;
            int id = actionModel.MaterialId;
            foreach (Renderer rndr in target)
            {
                if (rndr == null)
                    continue;

                try
                {
                    if (duration != 0)
                    {
#if !UNITY_EDITOR
                        rndr.sharedMaterials[id].DOColor(targetColor, "_BaseColor", duration);
#else
                        var rndrMaterial = rndr.materials[id];
                        rndrMaterial.DOKill();
                        rndrMaterial.DOColor(targetColor, "_BaseColor", duration);
#endif
                    }
                    else
                    {
#if !UNITY_EDITOR
                        rndr.sharedMaterials[id].SetColor("_BaseColor", targetColor);
#else
                        var rndrMaterial = rndr.materials[id];
                        rndrMaterial.DOKill();
                        rndrMaterial.SetColor("_BaseColor", targetColor);
#endif
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogError(e.Message);
                }
            }

        }

        private static void ExecuteUpdateMultiCanvasGroup(UpdateMultiCanvasGroupActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.CanvasGroups == null || actionModel.CanvasGroups.Count <= 0)
                return;
            foreach (CanvasGroup canvasGroup in actionModel.CanvasGroups)
            {
                if (canvasGroup == null)
                    continue;
                canvasGroup.alpha = actionModel.Alpha;
                canvasGroup.interactable = actionModel.IsInteractible;
                canvasGroup.blocksRaycasts = actionModel.BlockRaycasts;
            }
        }

        private static void ExecuteCanvasGroupInteraction(UpdateCanvasGroupInteractionActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.CanvasGroup == null)
                return;
            actionModel.CanvasGroup.interactable =
                actionModel.IsInteractible;
        }


        private void ExecuteStopAnimation(StopAnimationActionModel actionModel)
        {
            _animation.Rewind();
            _animation.Sample();
            _animation.Stop(actionModel.Clip);
        }

        // UI ACtions
        private void ExecuteSetText(SetTextActionModel actionModel)
        {
            TMP_Text textComponent = actionModel.TextComponent;
            if (textComponent != null)
            {
                textComponent.SetText(actionModel.Text);
            }
        }

        private void ExecuteSetTextColor(SetTextColorActionModel actionModel)
        {
            TMP_Text textComponent = actionModel.TextComponent;
            if (textComponent != null)
            {
                textComponent.color = actionModel.Color;
            }
        }


        private void ExecuteSetImageColor(SetImageColorActionModel actionModel)
        {
            Image imageComponent = actionModel.Image;
            if (imageComponent != null)
            {
                imageComponent.color = actionModel.Color;
            }
        }

        private void ExecuteSetGraphicsRaycaster(SetGraphicsRaycasterActionModel actionModel)
        {
            if (actionModel == null)
                return;
            if (actionModel.Raycasters == null || actionModel.Raycasters.Count <= 0)
                return;

            foreach (GraphicRaycaster graphicRaycaster in actionModel.Raycasters)
            {
                graphicRaycaster.enabled = actionModel.IsEnabled;
            }
        }

        public float GetStateDuration(string stateName)
        {
            if (_cacheData == null || !_cacheData.ContainsKey(stateName))
            {
                return 0;
            }

            return _cacheData[stateName].Duration;
        }

        public void StopAllAnimations()
        {
            if (_animation)
            {
                _animation.Stop();
            }
        }





        private void ExecuteSetRectTransformAction(SetRectTransformActionModel actionModel)
        {
            if (actionModel == null ||
                actionModel.Target == null)
                return;

            RectTransform target = actionModel.Target;
            if (target == null)
            {

                return;
            }

            target.sizeDelta = actionModel.WH;
            target.anchorMin = actionModel.AnchorMin;
            target.anchorMax = actionModel.AnchorMax;
            target.pivot = actionModel.Pivot;
            target.rotation = Quaternion.Euler(actionModel.Rotation);
            target.localScale = actionModel.Scale;
            target.anchoredPosition3D = actionModel.Position;

        }



        private void ExecuteSetColliderActive(SetColliderActiveActionModel actionModel)
        {
            if (actionModel == null)
            {
                return;
            }

            if (actionModel.Collider == null)
            {
                return;
            }

            actionModel.Collider.enabled = actionModel.Enabled;
        }

        public void ExecuteSetComponentEnabled(SetComponentEnabledActionModel actionModel)
        {
            if (actionModel == null)
            {
                return;
            }

            Behaviour targetComponent = actionModel.TargetComponent;
            if (targetComponent == null)
            {
                return;
            }

            targetComponent.enabled = actionModel.Enabled;
        }



        public void ExecutePlayAnimatorSafe(PlayAnimatorSafeActionModel playAnimatorSafeActionModel)
        {

            Animator animator = playAnimatorSafeActionModel.Animator;

            if (animator == null)
                return;

            switch (playAnimatorSafeActionModel.Type)
            {
                case AnimatorParameterType.Bool:
                    animator.SetBool(playAnimatorSafeActionModel.ParameterName, playAnimatorSafeActionModel.BoolValue);
                    break;
                case AnimatorParameterType.Int:
                    animator.SetInteger(playAnimatorSafeActionModel.ParameterName,
                        playAnimatorSafeActionModel.IntValue);
                    break;
                case AnimatorParameterType.Float:
                    animator.SetFloat(playAnimatorSafeActionModel.ParameterName,
                        playAnimatorSafeActionModel.FloatValue);
                    break;
                case AnimatorParameterType.Trigger:
                    animator.SetTrigger(playAnimatorSafeActionModel.TriggerName);
                    break;
                case AnimatorParameterType.State:
                    animator.Play(playAnimatorSafeActionModel.StateName);
                    break;

            }
        }

        public void ExecuteDoRotate(DoRotateActionModel doRotateAction)
        {
            if (doRotateAction == null || doRotateAction.Target == null)
            {
                return;
            }

            Transform target = doRotateAction.Target;
            if (target == null)
            {
                return;
            }

            target.DOKill(); // Kill any previous tween running on this transform, in case the state is called with force = true
            target.transform.rotation = Quaternion.identity;

            Tweener tween = target.DORotate(doRotateAction.Axis, doRotateAction.Duration, RotateMode.FastBeyond360)
                .SetLoops(doRotateAction.Loop ? -1 : 1).SetEase(Ease.Linear);
            _cachedTweens.Add(tween);

        }


        public void ExecuteDoShakePosition(DoShakePositionActionModel doShakePositionActionModel)
        {
            if (doShakePositionActionModel == null || doShakePositionActionModel.Target == null)
                return;
            if (DOTween.IsTweening(doShakePositionActionModel.Target))
                return;
            Transform target = doShakePositionActionModel.Target;
            if (target == null)
            {
                return;
            }

            target.DOKill();
            Tweener tween = target.DOShakePosition(doShakePositionActionModel.Duration,
                doShakePositionActionModel.Strength,
                doShakePositionActionModel.Vibaration, doShakePositionActionModel.Randomness,
                doShakePositionActionModel.FadeOut).SetLoops(doShakePositionActionModel.Loop ? -1 : 1);
            _cachedTweens.Add(tween);
        }


        public void ExecuteDoPunchScale(DoPunchScaleActionModel doPunchScaleActionModel)
        {
            if (doPunchScaleActionModel == null || doPunchScaleActionModel.Target == null)
                return;
            if (DOTween.IsTweening(doPunchScaleActionModel.Target))
                return;
            Transform target = doPunchScaleActionModel.Target;
            if (target == null)
            {
                return;
            }

            target.DOKill();
            Tweener tween = target.DOPunchScale(doPunchScaleActionModel.Punch, doPunchScaleActionModel.Duration,
                doPunchScaleActionModel.Vibaration, doPunchScaleActionModel.Elasticity);
            _cachedTweens.Add(tween);
        }


        public void ExecuteDoMove(DoMoveActionModel doMoveActionModel)
        {
            if (doMoveActionModel == null || doMoveActionModel.Target == null)
                return;

            Transform target = doMoveActionModel.Target;
            if (target == null)
            {
                return;
            }

            target.DOKill();

            Tweener tween = target
                .DOLocalMove(doMoveActionModel.MoveTo, doMoveActionModel.Duration, doMoveActionModel.Snapping)
                .SetEase(doMoveActionModel.EaseType).SetUpdate(true);
            _cachedTweens.Add(tween);
        }

        public void ExecuteDoFade(DoFadeActionModel doFadeActionModel)
        {
            if (doFadeActionModel == null || doFadeActionModel.Target == null)
                return;

            CanvasGroup target = doFadeActionModel.Target;
            if (target == null)
            {
                return;
            }

            target.DOKill();
            target.interactable = doFadeActionModel.IsInteractible;
            target.blocksRaycasts = doFadeActionModel.BlockRaycasts;
                Tweener tween = target.DOFade(doFadeActionModel.FadeTo, doFadeActionModel.Duration)
                .SetEase(doFadeActionModel.EaseType).SetUpdate(true);
            _cachedTweens.Add(tween);
        }

        public void ExecuteDoFadeGroup(DoFadeGroupActionModel doFadeGroupActionModel)
        {
            if (doFadeGroupActionModel == null || doFadeGroupActionModel.Targets == null ||
                doFadeGroupActionModel.Targets.Count <= 0)
                return;

            foreach (CanvasGroup target in doFadeGroupActionModel.Targets)
            {
                if (target == null)
                {
                    return;
                }

                target.DOKill();
                target.interactable = doFadeGroupActionModel.IsInteractible;
                target.blocksRaycasts = doFadeGroupActionModel.BlockRaycasts;
                Tweener tween = target.DOFade(doFadeGroupActionModel.FadeTo, doFadeGroupActionModel.Duration)
                    .SetEase(doFadeGroupActionModel.EaseType);
                _cachedTweens.Add(tween);
            }

        }

        public void ExecuteSetMaterial(SetMaterialActionModel setMaterialActionModel)
        {
            if (setMaterialActionModel == null)
                return;
            if (setMaterialActionModel.Image == null)
                return;
            setMaterialActionModel.Image.material = setMaterialActionModel.Material;
        }

        public void ExecutePlaySoundEvent(PlaySoundEventActionModel playSoundEventActionModel)
        {
            /*if (AudioController.Instance)
            {
                AudioController.Play(playSoundEventActionModel.EventName);
            }*/
        }

        public void ExecuteStopSoundEvent(StopSoundActionModel playSoundEventActionModel)
        {
            /*if (AudioController.Instance)
            {
                AudioController.Stop(playSoundEventActionModel.EventName);
            }*/
        }


    }

    [Serializable]
    public class SimpleStateModel
    {
        [SerializeReference] public List<IBasicActionModel> BasicActionModels;
        public string State;
        public int LayerIndex;
        public float CustomDuration;
        [NonSerialized] public float Duration;
    }

    [Serializable]
    public class SimpleStateActionModel
    {
        public ActionType ActionType;
        public AnimationActionModel AnimationActionModel;
        public SetSpriteActionModel SetSpriteActionModel;
        public UpdateCanvasGroupActionModel UpdateCanvasGroupActionModel;
        public MeshRendererActionModel MeshRendererActionModel;
        public ColliderActionModel ColliderActionModel;
        public SwapMaterialActionModel SwapMaterialActionModel;
        public SwapMaterialListActionModel SwapMaterialListActionModel;
        public SetActiveActionModel SetActiveActionModel;
        public SetActiveListActionModel SetActiveListActionModel;
        public SetTransformActionModel SetTransformActionModel;
        public AnimateColorActionModel AnimateColorActionModel;
        public AnimateSharedColorActionModel AnimateSharedColorActionModel;
        public SetPropertyActionModel SetPropertyActionModel;
        public SetGameCameraActionModel SetGameCameraActionModel;
        public UpdateMultiCanvasGroupActionModel UpdateMultiCanvasGroupActionModel;
        public UpdateCanvasGroupInteractionActionModel UpdateCanvasGroupInteractionActionModel;
        public StopAnimationActionModel StopAnimationActionModel;
        public SetTextActionModel SetTextActionModel;
        public SetTextColorActionModel SetTextColorActionModel;
        public SetImageColorActionModel SetImageColorActionModel;
        public SetGraphicsRaycasterActionModel SetGraphicsRaycasterActionModel;
        public SetRectPropertyActionModel SetRectPropertyActionModel;
        public SetTransformByNameActionModel SetTransformByNameActionModel;
        public SetRectTransformByNameActionModel SetRectTransformByNameActionModel;
        public PlayExternalStateModel PlayExternalStateModel;
        public PlaySoundEventActionModel PlaySoundEventActionModel;
        public SetComponentEnabledActionModel SetComponentEnabledActionModel;
        public SetRectTransformActionModel SetRectTransformActionModel;
        public PlayAnimatorActionModel PlayAnimatorActionModel;
        public SetColliderActiveActionModel SetColliderActiveActionModel;
        public PlayAnimatorSafeActionModel PlayAnimatorSafeActionModel;
        public StopSoundActionModel StopSoundActionModel;

    }

    public interface IBasicActionModel
    {
        void Dispose();
    }

    [Serializable]
    public class NoActionModel : IBasicActionModel
    {
        public void Dispose()
        {
        }
    }

    [Serializable]
    public class AnimationActionModel : IBasicActionModel
    {
        public string Clip;
        public float Weight = 0.5f;
        public int Layer;
        [NonSerialized] public float Length;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class SetSpriteActionModel : IBasicActionModel
    {
        public Image Image;
        public Sprite Sprite;

        public void Dispose()
        {
            Image = null;
            Sprite = null;
        }
    }

    [Serializable]
    public class UpdateCanvasGroupActionModel : IBasicActionModel
    {
        public CanvasGroup CanvasGroup;
        public float Alpha;
        public bool IsInteractible;
        public bool BlockRaycasts;

        public void Dispose()
        {
            CanvasGroup = null;
        }
    }

    [Serializable]
    public class MeshRendererActionModel : IBasicActionModel
    {
        public MeshRenderer MeshRenderer;
        public bool Enabled;

        public void Dispose()
        {
            MeshRenderer = null;
        }
    }

    [Serializable]
    public class ColliderActionModel : IBasicActionModel
    {
        public Collider Collider;
        public bool Enabled;

        public void Dispose()
        {
            Collider = null;
        }
    }

    [Serializable]
    public class SwapMaterialActionModel : IBasicActionModel
    {
        public Renderer Renderer;
        public int Index;
        public Material Material;

        public void Dispose()
        {
            Renderer = null;
            Material = null;
        }
    }

    [Serializable]
    public class SwapMaterialListActionModel : IBasicActionModel
    {
        public List<Renderer> Renderers;
        public int Index;
        public Material Material;

        public void Dispose()
        {
            Renderers.Clear();
            Material = null;
        }
    }

    [Serializable]
    public class SetActiveActionModel : IBasicActionModel
    {
        public GameObject Target;
        public bool State;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class SetActiveListActionModel : IBasicActionModel
    {
        public List<GameObject> Targets;
        public bool State;

        public void Dispose()
        {
            Targets.Clear();
        }
    }

    [Serializable]
    public class AnimateColorActionModel : IBasicActionModel
    {
        public List<Renderer> Target;
        public Color TargetColor;
        public int MaterialId;
        public float Duration;

        public void Dispose()
        {
            Target.Clear();
        }
    }

    [Serializable]
    public class AnimateSharedColorActionModel : IBasicActionModel
    {
        public List<Renderer> Target;
        public Color TargetColor;
        public int MaterialId;
        public float Duration;

        public void Dispose()
        {
            Target.Clear();
        }
    }

    [Serializable]
    public class SetTransformActionModel : IBasicActionModel
    {
        public GameObject Target;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;
        public bool IsLocal = false;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class SetTransformByNameActionModel : IBasicActionModel
    {
        public string TargetName;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class SetRectTransformByNameActionModel : IBasicActionModel
    {
        public string TargetName;
        public Vector3 Position;
        public Vector2 WH;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class SetRectTransformActionModel : IBasicActionModel
    {
        public RectTransform Target;
        public Vector3 Position;
        public Vector2 WH;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;

        public void Dispose()
        {
            Target = null;
        }
    }


    [Serializable]
    public class SetPropertyActionModel : IBasicActionModel
    {
        public List<Transform> Targets;
        public PropertyType TransformType;
        public float Value;

        public void Dispose()
        {
            Targets.Clear();
        }
    }

    [Serializable]
    public class SetRectPropertyActionModel : IBasicActionModel
    {
        public List<RectTransform> Targets;
        public PropertyType TransformType;
        public float Value;

        public void Dispose()
        {
            Targets.Clear();
        }
    }

    [Serializable]
    public class SetGameCameraActionModel : IBasicActionModel
    {
        public string CameraId;
        public bool IsActive;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class UpdateMultiCanvasGroupActionModel : IBasicActionModel
    {
        public List<CanvasGroup> CanvasGroups;
        public float Alpha;
        public bool IsInteractible;
        public bool BlockRaycasts;

        public void Dispose()
        {
            CanvasGroups.Clear();
        }
    }

    [Serializable]
    public class UpdateCanvasGroupInteractionActionModel : IBasicActionModel
    {
        public CanvasGroup CanvasGroup;
        public bool IsInteractible;

        public void Dispose()
        {
            CanvasGroup = null;
        }
    }

    [Serializable]
    public class StopAnimationActionModel : IBasicActionModel
    {
        public string Clip;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class SetGraphicsRaycasterActionModel : IBasicActionModel
    {
        public List<GraphicRaycaster> Raycasters;
        public bool IsEnabled;

        public void Dispose()
        {
            Raycasters.Clear();
        }
    }

    // UI ACTIONS
    [Serializable]
    public class SetTextActionModel : IBasicActionModel
    {
        public TMP_Text TextComponent;
        public string Text;

        public void Dispose()
        {
            TextComponent = null;
        }
    }


    [Serializable]
    public class SetTextColorActionModel : IBasicActionModel
    {
        public TMP_Text TextComponent;
        public Color Color;

        public void Dispose()
        {
            TextComponent = null;
        }
    }

    [Serializable]
    public class SetImageColorActionModel : IBasicActionModel
    {
        public Image Image;
        public Color Color;

        public void Dispose()
        {
            Image = null;
        }
    }



    [Serializable]
    public class PlayExternalStateModel : IBasicActionModel
    {
        public SimpleStateComponent Target;
        public string State;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class PlaySoundEventActionModel : IBasicActionModel
    {
        public string EventName;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class SetComponentEnabledActionModel : IBasicActionModel
    {
        public Behaviour TargetComponent;
        public bool Enabled;

        public void Dispose()
        {
            TargetComponent = null;
        }
    }


    [Serializable]
    public class PlayAnimatorActionModel : IBasicActionModel
    {
        public string Animator;
        public AnimatorParameterType Type;
        public string ParameterName;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public string StateName;
        public string TriggerName;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class PlayAnimatorSafeActionModel : IBasicActionModel
    {
        public Animator Animator;
        public AnimatorParameterType Type;
        public string ParameterName;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public string StateName;
        public string TriggerName;

        public void Dispose()
        {
        }
    }

    [Serializable]
    public class SetColliderActiveActionModel : IBasicActionModel
    {
        public Collider Collider;
        public bool Enabled;

        public void Dispose()
        {
            Collider = null;
        }
    }

    [Serializable]
    public class DoRotateActionModel : IBasicActionModel
    {
        public Transform Target;
        public Vector3 Axis;
        public float Duration;
        public bool Loop;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class DoShakePositionActionModel : IBasicActionModel
    {
        public Transform Target;
        public bool Loop;
        public float Duration;
        public Vector3 Strength;
        public int Vibaration;
        public float Randomness;
        public bool FadeOut;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class DoPunchScaleActionModel : IBasicActionModel
    {
        public Transform Target;
        public Vector3 Punch;
        public float Duration;
        public int Vibaration;
        public float Elasticity;
        public bool Snapping;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class DoMoveActionModel : IBasicActionModel
    {
        public Transform Target;
        public Vector3 MoveTo;
        public float Duration;
        public bool Snapping;
        public Ease EaseType;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class DoFadeActionModel : IBasicActionModel
    {
        public CanvasGroup Target;
        public float FadeTo;
        public float Duration;
        public bool IsInteractible;
        public bool BlockRaycasts;
        public Ease EaseType;

        public void Dispose()
        {
            Target = null;
        }
    }

    [Serializable]
    public class DoFadeGroupActionModel : IBasicActionModel
    {
        public List<CanvasGroup> Targets;
        public float FadeTo;
        public float Duration;
        public bool IsInteractible;
        public bool BlockRaycasts;
        public Ease EaseType;

        public void Dispose()
        {
            Targets = null;
        }
    }

    [Serializable]
    public class SetMaterialActionModel : IBasicActionModel
    {
        public Image Image;
        public Material Material;

        public void Dispose()
        {
            Image = null;
            Material = null;
        }
    }

    [Serializable]
    public class StopSoundActionModel : IBasicActionModel
    {
        public string EventName;
        public void Dispose()
        {
            EventName = string.Empty;
        }
    }

    [Serializable]
    public enum AnimatorParameterType
    {
        Bool,
        Int,
        Float,
        Trigger,
        State
    }



    [Serializable]
    public enum ActionType
    {
        NoAction,
        PlayAnimation,
        SetSprite,
        UpdateCanvasGroup,
        UpdateMeshRenderer,
        UpdateCollider,
        SwapMaterial,
        SetActive,
        SetTransform,
        AnimateColor,
        SetTransformProperty,
        SetGameCamera,
        UpdateMultiCanvasGroup,
        StopAnimation,
        SetText,
        SetTextColor,
        SetImageColor,
        SetGraphicsRaycasters,
        SetRectProperty,
        SwapMaterialList,
        AnimateSharedColor,
        UpdateCanvasGroupInteraction,
        SetTransformByName,
        SetRectTransformByName,
        PlayExternalState,
        SetActiveList,
        PlaySoundEvent,
        SetComponentEnabled,
        SetRectTransform,
        PlayAnimator,
        SetColliderActive,
        DoRotate,
        DoShakePosition,
        DoPunchScale,
        DoMove,
        DoFade,
        SetMaterial,
        DoFadeGroup,
        PlayAnimatorSafe,
        StopSound,
        // ALWAYS ADD NEW ACTIONS AT THE BOTTOM
    }

    [Serializable]
    public enum PropertyType
    {
        PositionX,
        PositionY,
        PositionZ,
        RotationX,
        RotationY,
        RotationZ,
        ScaleX,
        ScaleY,
        ScaleZ
    }
}
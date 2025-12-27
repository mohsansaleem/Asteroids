#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Oryx
{
    [CustomEditor(typeof(SimpleStateComponent))]
    public class FSimpleStateComponentEditor : Editor
    {
        private SerializedProperty _useDefaultState;
        private List<string> _animationClips;
        private const string STATE_CLIP_KEY = "FSimpleState";

        private void OnEnable()
        {
            _useDefaultState = serializedObject.FindProperty("UseDefaultState");
        }

        private Dictionary<SimpleStateModel, bool> _foldoutCache;
        private Dictionary<SimpleStateActionModel, int> _externalStateFoldoutOld;
        private Dictionary<IBasicActionModel, int> _externalStateFoldoutNew;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Simple State Component - CLOSED BETA", EditorStyles.boldLabel);
            SimpleStateComponent fSimpleStateComponent = (SimpleStateComponent)target;
            if (fSimpleStateComponent.SimpleStateModels == null)
                fSimpleStateComponent.SimpleStateModels = new List<SimpleStateModel>();

            List<SimpleStateModel> simpleStateModels = fSimpleStateComponent.SimpleStateModels;
            /*DrawVersioned(fSimpleStateComponent, simpleStateModels, 0);
            _drawVersion1 = GUILayout.Toggle(_drawVersion1,"Version 1 Dont Change","Foldout",GUILayout.ExpandWidth(false));
            if(_drawVersion1)
                */
            DrawVersioned(fSimpleStateComponent, simpleStateModels);
        }

        private void DrawVersioned(SimpleStateComponent fSimpleStateComponent, List<SimpleStateModel> simpleStateModels)
        {
            _animationClips = new List<string>();
            if (_foldoutCache == null)
            {
                _foldoutCache = new Dictionary<SimpleStateModel, bool>();
            }

            if (_externalStateFoldoutOld == null)
            {
                _externalStateFoldoutOld = new Dictionary<SimpleStateActionModel, int>();
            }

            if (_externalStateFoldoutNew == null)
            {
                _externalStateFoldoutNew = new Dictionary<IBasicActionModel, int>();
            }


            Animation component = fSimpleStateComponent.GetComponent<Animation>();
            if (component != null && !component.Equals(null))
            {
                foreach (AnimationState animationState in component)
                {
                    _animationClips.Add(animationState.clip.name);
                }
            }

            if (fSimpleStateComponent.SimpleStateModels == null)
            {
                fSimpleStateComponent.SimpleStateModels = new List<SimpleStateModel>();
            }
            

            EditorGUILayout.LabelField("States", EditorStyles.boldLabel);
            int itemToRemoveFromIndex = -1;
            GenericMenu menu = new GenericMenu();
            for (var index = 0; index < simpleStateModels.Count; index++)
            {
                SimpleStateModel simpleStateModel = simpleStateModels[index];
                if (!_foldoutCache.ContainsKey(simpleStateModel))
                {
                    _foldoutCache[simpleStateModel] = string.IsNullOrEmpty(simpleStateModel.State);
                }

                EditorGUILayout.BeginHorizontal();
                _foldoutCache[simpleStateModel] = GUILayout.Toggle(_foldoutCache[simpleStateModel],
                    "State : " + (string.IsNullOrEmpty(simpleStateModel.State)
                        ? "[---State Name Missing---]"
                        : simpleStateModel.State), "Foldout", GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Play State", GUILayout.Width(100)))
                {
                    fSimpleStateComponent.PlayState(simpleStateModel.State, false, true);
                }

                EditorGUILayout.EndHorizontal();
                if (_foldoutCache[simpleStateModel])
                {
                    Rect stateRect = EditorGUILayout.BeginVertical("window");
                    DrawEachElement(simpleStateModel, menu, fSimpleStateComponent);
                    if (GUILayout.Button("Remove State"))
                    {
                        itemToRemoveFromIndex = index;
                    }

                    DrawContextMenu(stateRect, simpleStateModel, menu);
                    EditorGUILayout.EndVertical();
                }
            }

            if (menu.GetItemCount() > 0)
            {
                menu.ShowAsContext();
            }

            if (itemToRemoveFromIndex != -1)
            {
                var simpleStateModelToRemove = simpleStateModels[itemToRemoveFromIndex];
                simpleStateModels.RemoveAt(itemToRemoveFromIndex);
                if (_foldoutCache.ContainsKey(simpleStateModelToRemove))
                    _foldoutCache.Remove(simpleStateModelToRemove);
            }


            if (GUILayout.Button("Add New State"))
            {
                simpleStateModels.Add(new SimpleStateModel());
            }

            /*if (GUILayout.Button("Stop All Animations"))
            {
                fSimpleAnimationComponent.StopAllAnimations();
            }*/
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(fSimpleStateComponent);
            }
        }

        private static void DrawContextMenu(Rect stateRect, SimpleStateModel simpleStateModel, GenericMenu menu)
        {
            if (Event.current.type == EventType.ContextClick)
            {
                Vector2 mousePos = Event.current.mousePosition;
                if (stateRect.Contains(mousePos))
                {
                    string systemCopyBuffer = EditorGUIUtility.systemCopyBuffer;
                    if (!string.IsNullOrEmpty(systemCopyBuffer))
                    {
                        SimpleStateModel copiedData = null;
                        try
                        {
                            copiedData = JsonUtility.FromJson<SimpleStateModel>(systemCopyBuffer);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }

                        if (menu == null)
                        {
                            menu = new GenericMenu();
                        }

                        menu.AddItem(new GUIContent("Copy State"), false,
                            userData => { EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(userData); },
                            simpleStateModel);
                        if (copiedData != null)
                        {
                            menu.AddItem(new GUIContent("Paste State"), false, userData =>
                            {
                                SimpleStateModel model = (SimpleStateModel)userData;
                                model.State = copiedData.State;
                                model.BasicActionModels = copiedData.BasicActionModels;
                                model.LayerIndex = copiedData.LayerIndex;
                            }, simpleStateModel);
                        }

                        Event.current.Use();
                    }
                }
            }
        }

        private void DrawEachElement(SimpleStateModel simpleStateModel, GenericMenu menu,
            SimpleStateComponent simpleStateComponent)
        {
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.Toggle();
            simpleStateModel.State =
                EditorGUILayout.TextField(simpleStateModel.State, GUILayout.Width(Screen.width * 0.3f));
            if (GUILayout.Button("Play State"))
            {
                SimpleStateComponent fSimpleStateComponent = (SimpleStateComponent)target;
                fSimpleStateComponent.PlayState(simpleStateModel.State, false, true);
            }

            EditorGUILayout.EndHorizontal();
            simpleStateModel.CustomDuration =
                EditorGUILayout.FloatField("Custom Duration", simpleStateModel.CustomDuration);

            int itemToAddAfterIndex = -1;
            int itemToRemoveFromIndex = -1;

            if (simpleStateModel.BasicActionModels == null)
            {
                simpleStateModel.BasicActionModels = new List<IBasicActionModel>();
            }

            if (simpleStateModel.BasicActionModels.Count == 0)
            {
                simpleStateModel.BasicActionModels.Add(new NoActionModel());
            }

            for (var index = 0; index < simpleStateModel.BasicActionModels.Count; index++)
            {
                EditorGUILayout.BeginHorizontal();
                IBasicActionModel simpleStateActionModel = simpleStateModel.BasicActionModels[index];
                if (simpleStateActionModel == null)
                {
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                
                Type type = simpleStateActionModel.GetType();
                ActionType storedActionType = SimpleStateComponent.TypeToEnum[type];
                ActionType selectedActionType = (ActionType)EditorGUILayout.EnumPopup(storedActionType);
                Type selectedType = SimpleStateComponent.EnumToType[selectedActionType];
                if (storedActionType != selectedActionType)
                {
                    simpleStateActionModel = (IBasicActionModel)Activator.CreateInstance(selectedType);
                    simpleStateModel.BasicActionModels[index] = simpleStateActionModel;
                }

                GUI.backgroundColor = Color.cyan;
                if (GUILayout.Button("+"))
                {
                    itemToAddAfterIndex = index;
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("-"))
                {
                    itemToRemoveFromIndex = index;
                }

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
                simpleStateActionModel = DrawEachStateAction(simpleStateActionModel, menu, simpleStateComponent);
            }

            if (itemToAddAfterIndex != -1)
            {
                simpleStateModel.BasicActionModels.Insert(itemToAddAfterIndex + 1, new NoActionModel());
            }
            else if (itemToRemoveFromIndex != -1)
            {
                if (simpleStateModel.BasicActionModels.Count <= 1)
                    return;

                simpleStateModel.BasicActionModels.RemoveAt(itemToRemoveFromIndex);
            }
        }

        private SimpleStateActionModel DrawEachStateAction(SimpleStateActionModel stateActionModel, GenericMenu menu,
            SimpleStateComponent simpleStateComponent)
        {
            Rect stateRect = EditorGUILayout.BeginHorizontal("Box");
            SimpleStateActionModel result = stateActionModel;

            switch (stateActionModel.ActionType)
            {
                case ActionType.PlayAnimation:
                    DrawAnimationAction(stateActionModel.AnimationActionModel);
                    break;

                case ActionType.StopAnimation:
                    DrawStopAnimationAction(stateActionModel.StopAnimationActionModel);
                    break;
                case ActionType.SetSprite:
                    DrawSetSpriteAction(stateActionModel.SetSpriteActionModel);
                    break;
                case ActionType.UpdateCanvasGroup:
                    DrawUpdateCanvasAction(stateActionModel.UpdateCanvasGroupActionModel);
                    break;
                case ActionType.UpdateMeshRenderer:
                    DrawUpdateMeshRendererAction(stateActionModel.MeshRendererActionModel);
                    break;
                case ActionType.UpdateCollider:
                    DrawUpdateColliderAction(stateActionModel.ColliderActionModel);
                    break;
                case ActionType.SwapMaterial:
                    DrawSwapMaterialAction(stateActionModel.SwapMaterialActionModel);
                    break;
                case ActionType.SwapMaterialList:
                    DrawSwapMaterialListAction(stateActionModel.SwapMaterialListActionModel);
                    break;
                case ActionType.SetActive:
                    DrawSetActiveAction(stateActionModel.SetActiveActionModel);
                    break;
                case ActionType.SetActiveList:
                    DrawSetActiveAction(stateActionModel.SetActiveListActionModel);
                    break;
                case ActionType.SetTransform:
                    DrawSetTransformAction(stateActionModel.SetTransformActionModel);
                    break;
                case ActionType.AnimateColor:
                    DrawAnimationColorAction(stateActionModel.AnimateColorActionModel);
                    break;
                case ActionType.AnimateSharedColor:
                    DrawAnimateSharedAction(stateActionModel.AnimateSharedColorActionModel);
                    break;
                case ActionType.SetTransformProperty:
                    DrawSetTransformPropertyAction(stateActionModel.SetPropertyActionModel);
                    break;
                case ActionType.SetRectProperty:
                    DrawSetRectPropertyAction(stateActionModel.SetRectPropertyActionModel);
                    break;
                case ActionType.SetGameCamera:
                    DrawSetGameCameraAction(stateActionModel.SetGameCameraActionModel);
                    break;
                // UI related Actions
                case ActionType.SetText:
                    DrawSetTextAction(stateActionModel.SetTextActionModel);
                    break;
                case ActionType.SetTextColor:
                    DrawSetTextColorAction(stateActionModel.SetTextColorActionModel);
                    break;
                case ActionType.SetImageColor:
                    DrawSetImageColorAction(stateActionModel.SetImageColorActionModel);
                    break;
                case ActionType.SetGraphicsRaycasters:
                    DrawSetGraphicsRaycasters(stateActionModel.SetGraphicsRaycasterActionModel);
                    break;
                case ActionType.UpdateMultiCanvasGroup:
                    DrawUpdateMultiCanvasGroupAction(stateActionModel.UpdateMultiCanvasGroupActionModel);
                    break;

                case ActionType.UpdateCanvasGroupInteraction:
                    DrawUpdateCanvasGroupInteractionAction(stateActionModel.UpdateCanvasGroupInteractionActionModel);
                    break;
                case ActionType.SetRectTransformByName:
                    DrawSetRectTransformByNameAction(menu, stateActionModel.SetRectTransformByNameActionModel, stateRect);
                    break;
                case ActionType.SetRectTransform:
                    DrawSetRectTransformAction(menu, stateActionModel.SetRectTransformActionModel, stateRect);
                    break;
                case ActionType.SetTransformByName:
                    DrawSetTransformByNameAction(menu, stateActionModel.SetTransformByNameActionModel, stateRect);
                    break;
                case ActionType.PlayExternalState:
                    DrawPlayExternalStateAction(stateActionModel, stateActionModel.PlayExternalStateModel);
                    break;
                case ActionType.PlaySoundEvent:
                    DrawPlaySoundEventAction(stateActionModel.PlaySoundEventActionModel);
                    break;
                case ActionType.SetComponentEnabled:
                    DrawSetComponentEnabledAction(stateActionModel.SetComponentEnabledActionModel);
                    break;
                case ActionType.PlayAnimator:
                    DrawPlayAnimatorAction(stateActionModel.PlayAnimatorActionModel);
                    break;
                case ActionType.SetColliderActive:
                    DrawSetColliderAction(stateActionModel.SetColliderActiveActionModel);
                    break;
                case ActionType.PlayAnimatorSafe:
                    DrawPlayAnimatorSafeAction(stateActionModel.PlayAnimatorSafeActionModel);
                    break;
                case ActionType.StopSound:
                    DrawStopSoundEventAction(stateActionModel.StopSoundActionModel);
                    break;
            }

            EditorGUILayout.EndHorizontal();
            return result;
        }

        private IBasicActionModel DrawEachStateAction(IBasicActionModel basicActionModel, GenericMenu menu,
            SimpleStateComponent simpleStateComponent)
        {
            Rect stateRect = EditorGUILayout.BeginHorizontal("Box");
            IBasicActionModel result = basicActionModel;

            switch (basicActionModel)
            {
                case AnimationActionModel animationActionModel:
                    DrawAnimationAction(animationActionModel);
                    break;
                case SetSpriteActionModel setSpriteActionModel:
                    DrawSetSpriteAction(setSpriteActionModel);
                    break;
                case UpdateCanvasGroupActionModel updateCanvasGroupActionModel:
                    DrawUpdateCanvasAction(updateCanvasGroupActionModel);
                    break;
                case MeshRendererActionModel meshRendererActionModel:
                    DrawUpdateMeshRendererAction(meshRendererActionModel);
                    break;
                case ColliderActionModel colliderActionModel:
                    DrawUpdateColliderAction(colliderActionModel);
                    break;
                case SwapMaterialActionModel swapMaterialActionModel:
                    DrawSwapMaterialAction(swapMaterialActionModel);
                    break;
                case SwapMaterialListActionModel swapMaterialListActionModel:
                    DrawSwapMaterialListAction(swapMaterialListActionModel);
                    break;
                case SetActiveActionModel setActiveActionModel:
                    DrawSetActiveAction(setActiveActionModel);
                    break;
                case SetActiveListActionModel setActiveListActionModel:
                    DrawSetActiveAction(setActiveListActionModel);
                    break;
                case SetTransformActionModel setTransformActionModel:
                    DrawSetTransformAction(setTransformActionModel);
                    break;
                case AnimateColorActionModel animateColorActionModel:
                    DrawAnimationColorAction(animateColorActionModel);
                    break;
                case AnimateSharedColorActionModel animateSharedColorActionModel:
                    DrawAnimateSharedAction(animateSharedColorActionModel);
                    break;
                case SetPropertyActionModel setPropertyActionModel:
                    DrawSetTransformPropertyAction(setPropertyActionModel);
                    break;
                case SetRectPropertyActionModel setRectPropertyActionModel:
                    DrawSetRectPropertyAction(setRectPropertyActionModel);
                    break;
                case SetGameCameraActionModel setGameCameraActionModel:
                    DrawSetGameCameraAction(setGameCameraActionModel);
                    break;
                // UI related Actions
                case SetTextActionModel setTextActionModel:
                    DrawSetTextAction(setTextActionModel);
                    break;
                case SetTextColorActionModel setTextColorActionModel:
                    DrawSetTextColorAction(setTextColorActionModel);
                    break;
                case SetImageColorActionModel setImageColorActionModel:
                    DrawSetImageColorAction(setImageColorActionModel);
                    break;
                case SetGraphicsRaycasterActionModel setGraphicsRaycasterActionModel:
                    DrawSetGraphicsRaycasters(setGraphicsRaycasterActionModel);
                    break;
                case UpdateMultiCanvasGroupActionModel updateMultiCanvasGroupActionModel:
                    DrawUpdateMultiCanvasGroupAction(updateMultiCanvasGroupActionModel);
                    break;

                case UpdateCanvasGroupInteractionActionModel updateCanvasGroupInteractionActionModel:
                    DrawUpdateCanvasGroupInteractionAction(updateCanvasGroupInteractionActionModel);
                    break;
                case StopAnimationActionModel stopAnimationActionModel:
                    DrawStopAnimationAction(stopAnimationActionModel);
                    break;
                case SetRectTransformByNameActionModel setRectTransformByNameActionModel:
                    DrawSetRectTransformByNameAction(menu, setRectTransformByNameActionModel, stateRect);
                    break;
                case SetRectTransformActionModel setRectTransformActionModel:
                    DrawSetRectTransformAction(menu, setRectTransformActionModel, stateRect);
                    break;
                case SetTransformByNameActionModel setTransformByNameActionModel:
                    DrawSetTransformByNameAction(menu, setTransformByNameActionModel, stateRect);
                    break;
                case PlayExternalStateModel playExternalStateModel:
                    DrawPlayExternalStateAction(basicActionModel, playExternalStateModel);
                    break;
                case PlaySoundEventActionModel playSoundEventActionModel:
                    DrawPlaySoundEventAction(playSoundEventActionModel);
                    break;
                case SetComponentEnabledActionModel setComponentEnabledActionModel:
                    DrawSetComponentEnabledAction(setComponentEnabledActionModel);
                    break;
                case PlayAnimatorActionModel playAnimatorActionModel:
                    DrawPlayAnimatorAction(playAnimatorActionModel);
                    break;
                case SetColliderActiveActionModel setColliderActiveActionModel:
                    DrawSetColliderAction(setColliderActiveActionModel);
                    break;
                case DoRotateActionModel doRotateActionModel:
                    DrawDoRotateAction(doRotateActionModel);
                    break;
                case DoShakePositionActionModel doShakePositionActionModel:
                    DrawDoShakePositionAction(doShakePositionActionModel);
                    break;
                case DoPunchScaleActionModel doPunchScaleActionModel:
                    DrawDoPunchScaleAction(doPunchScaleActionModel);
                    break;
                case DoMoveActionModel doMoveActionModel:
                    DrawDoMoveAction(doMoveActionModel);
                    break;
                case DoFadeActionModel doFadeActionModel:
                    DrawDoFadeAction(doFadeActionModel);
                    break;
                case SetMaterialActionModel setMaterialActionModel:
                    DrawSetMaterialAction(setMaterialActionModel);
                    break;
                case DoFadeGroupActionModel doFadeGroupActionModel:
                    DrawDoFadeGroupAction(doFadeGroupActionModel);
                    break;
                case PlayAnimatorSafeActionModel playAnimatorSafeActionModel:
                    DrawPlayAnimatorSafeAction(playAnimatorSafeActionModel);
                    break;
                case StopSoundActionModel stopSoundActionModel:
                    DrawStopSoundEventAction(stopSoundActionModel);
                    break;

            }

            EditorGUILayout.EndHorizontal();
            return result;
        }

        private static void DrawPlayAnimatorAction(PlayAnimatorActionModel playAnimatorActionModel)
        {
            EditorGUILayout.BeginVertical();
            playAnimatorActionModel.Type = (AnimatorParameterType)EditorGUILayout.EnumPopup(playAnimatorActionModel.Type);
            switch (playAnimatorActionModel.Type)
            {
                case AnimatorParameterType.Bool:
                    playAnimatorActionModel.Animator =
                        EditorGUILayout.TextField("Animator ", playAnimatorActionModel.Animator);
                    playAnimatorActionModel.ParameterName =
                        EditorGUILayout.TextField("Parameter ", playAnimatorActionModel.ParameterName);
                    playAnimatorActionModel.BoolValue =
                        EditorGUILayout.Toggle("Enabled: ", playAnimatorActionModel.BoolValue);
                    break;
                case AnimatorParameterType.Int:
                    playAnimatorActionModel.Animator =
                        EditorGUILayout.TextField("Animator ", playAnimatorActionModel.Animator);
                    playAnimatorActionModel.ParameterName =
                        EditorGUILayout.TextField("Parameter ", playAnimatorActionModel.ParameterName);
                    playAnimatorActionModel.IntValue =
                        EditorGUILayout.IntField("Value: ", playAnimatorActionModel.IntValue);
                    break;
                case AnimatorParameterType.Float:
                    playAnimatorActionModel.Animator =
                        EditorGUILayout.TextField("Animator ", playAnimatorActionModel.Animator);
                    playAnimatorActionModel.ParameterName =
                        EditorGUILayout.TextField("Parameter ", playAnimatorActionModel.ParameterName);
                    playAnimatorActionModel.FloatValue =
                        EditorGUILayout.FloatField("Value: ", playAnimatorActionModel.FloatValue);
                    break;
                case AnimatorParameterType.Trigger:
                    playAnimatorActionModel.Animator =
                        EditorGUILayout.TextField("Animator ", playAnimatorActionModel.Animator);
                    playAnimatorActionModel.TriggerName =
                        EditorGUILayout.TextField("Trigger ", playAnimatorActionModel.TriggerName);
                    break;
                case AnimatorParameterType.State:
                    playAnimatorActionModel.Animator =
                        EditorGUILayout.TextField("Animator ", playAnimatorActionModel.Animator);
                    playAnimatorActionModel.StateName =
                        EditorGUILayout.TextField("State ", playAnimatorActionModel.StateName);
                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private static void DrawPlayAnimatorSafeAction(PlayAnimatorSafeActionModel playAnimatorActionModel)
        {
            EditorGUILayout.BeginVertical();
            playAnimatorActionModel.Type = (AnimatorParameterType)EditorGUILayout.EnumPopup(playAnimatorActionModel.Type);
            switch (playAnimatorActionModel.Type)
            {
                case AnimatorParameterType.Bool:
                    playAnimatorActionModel.Animator = (Animator)EditorGUILayout.ObjectField("Animator",
                        playAnimatorActionModel.Animator, typeof(Animator), true);
                    playAnimatorActionModel.ParameterName =
                        EditorGUILayout.TextField("Parameter ", playAnimatorActionModel.ParameterName);
                    playAnimatorActionModel.BoolValue =
                        EditorGUILayout.Toggle("Enabled: ", playAnimatorActionModel.BoolValue);
                    break;
                case AnimatorParameterType.Int:
                    playAnimatorActionModel.Animator = (Animator)EditorGUILayout.ObjectField("Animator",
                        playAnimatorActionModel.Animator, typeof(Animator), true);
                    playAnimatorActionModel.ParameterName =
                        EditorGUILayout.TextField("Parameter ", playAnimatorActionModel.ParameterName);
                    playAnimatorActionModel.IntValue =
                        EditorGUILayout.IntField("Value: ", playAnimatorActionModel.IntValue);
                    break;
                case AnimatorParameterType.Float:
                    playAnimatorActionModel.Animator = (Animator)EditorGUILayout.ObjectField("Animator",
                        playAnimatorActionModel.Animator, typeof(Animator), true);
                    playAnimatorActionModel.ParameterName =
                        EditorGUILayout.TextField("Parameter ", playAnimatorActionModel.ParameterName);
                    playAnimatorActionModel.FloatValue =
                        EditorGUILayout.FloatField("Value: ", playAnimatorActionModel.FloatValue);
                    break;
                case AnimatorParameterType.Trigger:
                    playAnimatorActionModel.Animator = (Animator)EditorGUILayout.ObjectField("Animator",
                        playAnimatorActionModel.Animator, typeof(Animator), true);
                    playAnimatorActionModel.TriggerName =
                        EditorGUILayout.TextField("Trigger ", playAnimatorActionModel.TriggerName);
                    break;
                case AnimatorParameterType.State:
                    playAnimatorActionModel.Animator = (Animator)EditorGUILayout.ObjectField("Animator",
                        playAnimatorActionModel.Animator, typeof(Animator), true);
                    playAnimatorActionModel.StateName =
                        EditorGUILayout.TextField("State ", playAnimatorActionModel.StateName);
                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private static void DrawSetComponentEnabledAction(SetComponentEnabledActionModel setComponentEnabledActionModel)
        {
            EditorGUILayout.BeginVertical();
            setComponentEnabledActionModel.TargetComponent = (Behaviour)EditorGUILayout.ObjectField("Target",
                setComponentEnabledActionModel.TargetComponent, typeof(Behaviour), true);
            setComponentEnabledActionModel.Enabled =
                EditorGUILayout.Toggle("Enabled: ", setComponentEnabledActionModel.Enabled);
            EditorGUILayout.EndVertical();
        }

        private static void DrawPlaySoundEventAction(PlaySoundEventActionModel playSoundEventActionModel)
        {
            EditorGUILayout.BeginVertical();
            playSoundEventActionModel.EventName =
                EditorGUILayout.TextField("Event Name", playSoundEventActionModel.EventName);
            EditorGUILayout.EndVertical();
        }

        private static void DrawStopSoundEventAction(StopSoundActionModel stopSoundActionModel)
        {
            EditorGUILayout.BeginVertical();
            stopSoundActionModel.EventName =
                EditorGUILayout.TextField("Event Name", stopSoundActionModel.EventName);
            EditorGUILayout.EndVertical();
        }

        private void DrawPlayExternalStateAction(SimpleStateActionModel stateActionModel,
            PlayExternalStateModel playExternalStateModel)
        {
            EditorGUILayout.BeginVertical();
            List<string> stateNames = new List<string>();
            playExternalStateModel.Target = (SimpleStateComponent)EditorGUILayout.ObjectField("Target",
                playExternalStateModel.Target, typeof(SimpleStateComponent), true);
            if (playExternalStateModel.Target != null)
            {
                foreach (SimpleStateModel stateModel in playExternalStateModel.Target.SimpleStateModels)
                {
                    stateNames.Add(stateModel.State);
                }

                int stateIndex = 0;
                if (string.IsNullOrEmpty(playExternalStateModel.State))
                {
                    if (_externalStateFoldoutOld.ContainsKey(stateActionModel))
                    {
                        stateIndex = _externalStateFoldoutOld[stateActionModel];
                    }
                    else
                    {
                        _externalStateFoldoutOld.Add(stateActionModel, stateIndex);
                    }
                }
                else
                {
                    stateIndex = stateNames.IndexOf(playExternalStateModel.State);
                }

                stateIndex = EditorGUILayout.Popup("States", stateIndex, stateNames.ToArray());
                playExternalStateModel.State = stateNames[stateIndex];
                _externalStateFoldoutOld[stateActionModel] = stateIndex;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPlayExternalStateAction(IBasicActionModel actionModel,
            PlayExternalStateModel playExternalStateModel)
        {
            EditorGUILayout.BeginVertical();
            List<string> stateNames = new List<string>();
            playExternalStateModel.Target = (SimpleStateComponent)EditorGUILayout.ObjectField("Target",
                playExternalStateModel.Target, typeof(SimpleStateComponent), true);
            if (playExternalStateModel.Target != null)
            {
                foreach (SimpleStateModel stateModel in playExternalStateModel.Target.SimpleStateModels)
                {
                    stateNames.Add(stateModel.State);
                }

                int stateIndex = 0;
                if (string.IsNullOrEmpty(playExternalStateModel.State))
                {
                    if (_externalStateFoldoutNew.ContainsKey(actionModel))
                    {
                        stateIndex = _externalStateFoldoutNew[actionModel];
                    }
                    else
                    {
                        _externalStateFoldoutNew.Add(actionModel, stateIndex);
                    }
                }
                else
                {
                    stateIndex = stateNames.IndexOf(playExternalStateModel.State);
                }

                if (stateIndex == -1)
                {
                    stateIndex = 0;
                }

                stateIndex = EditorGUILayout.Popup("States", stateIndex, stateNames.ToArray());
                playExternalStateModel.State = stateNames[stateIndex];
                _externalStateFoldoutNew[actionModel] = stateIndex;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSetTransformByNameAction(GenericMenu menu,
            SetTransformByNameActionModel setTransformByNameActionModel,
            Rect stateRect)
        {
            EditorGUILayout.BeginVertical();
            setTransformByNameActionModel.TargetName =
                EditorGUILayout.TextField("TargetName", setTransformByNameActionModel.TargetName);
            setTransformByNameActionModel.Position =
                EditorGUILayout.Vector3Field("Position", setTransformByNameActionModel.Position);
            setTransformByNameActionModel.Rotation =
                EditorGUILayout.Vector3Field("Rotation", setTransformByNameActionModel.Rotation);
            setTransformByNameActionModel.Scale =
                EditorGUILayout.Vector3Field("Scale", setTransformByNameActionModel.Scale);
            DrawContextMenu(stateRect, setTransformByNameActionModel, menu);
            EditorGUILayout.EndVertical();
        }

        private void DrawSetRectTransformAction(GenericMenu menu, SetRectTransformActionModel setRectTransformActionModel,
            Rect stateRect)
        {
            EditorGUILayout.BeginVertical();
            setRectTransformActionModel.Target = (RectTransform)EditorGUILayout.ObjectField("Target",
                setRectTransformActionModel.Target, typeof(RectTransform), true);
            setRectTransformActionModel.Position =
                EditorGUILayout.Vector3Field("Position", setRectTransformActionModel.Position);
            setRectTransformActionModel.WH = EditorGUILayout.Vector2Field("WH", setRectTransformActionModel.WH);
            setRectTransformActionModel.AnchorMin =
                EditorGUILayout.Vector2Field("AnchorMin", setRectTransformActionModel.AnchorMin);
            setRectTransformActionModel.AnchorMax =
                EditorGUILayout.Vector2Field("AnchorMax", setRectTransformActionModel.AnchorMax);
            setRectTransformActionModel.Pivot = EditorGUILayout.Vector2Field("Pivot", setRectTransformActionModel.Pivot);
            setRectTransformActionModel.Rotation =
                EditorGUILayout.Vector3Field("Rotation", setRectTransformActionModel.Rotation);
            setRectTransformActionModel.Scale = EditorGUILayout.Vector3Field("Scale", setRectTransformActionModel.Scale);
            DrawContextMenu(stateRect, setRectTransformActionModel, menu);
            EditorGUILayout.EndVertical();
        }

        private void DrawSetRectTransformByNameAction(GenericMenu menu,
            SetRectTransformByNameActionModel setRectTransformByNameActionModel, Rect stateRect)
        {
            EditorGUILayout.BeginVertical();
            setRectTransformByNameActionModel.TargetName =
                EditorGUILayout.TextField("TargetName", setRectTransformByNameActionModel.TargetName);
            setRectTransformByNameActionModel.Position =
                EditorGUILayout.Vector3Field("Position", setRectTransformByNameActionModel.Position);
            setRectTransformByNameActionModel.WH = EditorGUILayout.Vector2Field("WH", setRectTransformByNameActionModel.WH);
            setRectTransformByNameActionModel.AnchorMin =
                EditorGUILayout.Vector2Field("AnchorMin", setRectTransformByNameActionModel.AnchorMin);
            setRectTransformByNameActionModel.AnchorMax =
                EditorGUILayout.Vector2Field("AnchorMax", setRectTransformByNameActionModel.AnchorMax);
            setRectTransformByNameActionModel.Pivot =
                EditorGUILayout.Vector2Field("Pivot", setRectTransformByNameActionModel.Pivot);
            setRectTransformByNameActionModel.Rotation =
                EditorGUILayout.Vector3Field("Rotation", setRectTransformByNameActionModel.Rotation);
            setRectTransformByNameActionModel.Scale =
                EditorGUILayout.Vector3Field("Scale", setRectTransformByNameActionModel.Scale);
            DrawContextMenu(stateRect, setRectTransformByNameActionModel, menu);
            EditorGUILayout.EndVertical();
        }



        private static void DrawUpdateCanvasGroupInteractionAction(
            UpdateCanvasGroupInteractionActionModel updateCanvasGroupInteractionActionModel)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            updateCanvasGroupInteractionActionModel.CanvasGroup =
                (CanvasGroup)EditorGUILayout.ObjectField(updateCanvasGroupInteractionActionModel.CanvasGroup,
                    typeof(CanvasGroup), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            updateCanvasGroupInteractionActionModel.IsInteractible =
                EditorGUILayout.Toggle("Interactible", updateCanvasGroupInteractionActionModel.IsInteractible);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawUpdateMultiCanvasGroupAction(UpdateMultiCanvasGroupActionModel updateMultiCanvasGroupActionModel)
        {
            if (updateMultiCanvasGroupActionModel.CanvasGroups == null ||
                updateMultiCanvasGroupActionModel.CanvasGroups.Count <= 1)
            {
                updateMultiCanvasGroupActionModel.CanvasGroups = new List<CanvasGroup>() { null };
            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("F|All"))
            {
                updateMultiCanvasGroupActionModel.CanvasGroups = ((SimpleStateComponent)target)
                    .GetComponentsInChildren<CanvasGroup>().ToList();
            }

            if (GUILayout.Button("F|W|Canvas Only"))
            {
                Canvas[] canvases = ((SimpleStateComponent)target).GetComponentsInChildren<Canvas>();
                if (canvases.Length > 0)
                {
                    List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
                    foreach (Canvas canvase in canvases)
                    {
                        CanvasGroup canvasGroup = canvase.GetComponent<CanvasGroup>();
                        if (canvasGroup != null)
                        {
                            canvasGroups.Add(canvasGroup);
                        }
                    }

                    if (canvasGroups.Count > 0)
                        updateMultiCanvasGroupActionModel.CanvasGroups = canvasGroups;
                    else
                        updateMultiCanvasGroupActionModel.CanvasGroups = new List<CanvasGroup>() { null };
                }
            }

            if (GUILayout.Button("Clear All"))
            {
                updateMultiCanvasGroupActionModel.CanvasGroups = new List<CanvasGroup>() { null };
            }

            EditorGUILayout.EndHorizontal();

            int itemToAddAfterIndex = -1;
            int itemToRemoveFromIndex = -1;
            for (var index = 0; index < updateMultiCanvasGroupActionModel.CanvasGroups.Count; index++)
            {
                CanvasGroup canvasGroup = updateMultiCanvasGroupActionModel.CanvasGroups[index];
                EditorGUILayout.BeginHorizontal();
                canvasGroup = (CanvasGroup)EditorGUILayout.ObjectField(canvasGroup, typeof(CanvasGroup), true);
                if (GUILayout.Button("+"))
                {
                    itemToAddAfterIndex = index;
                }

                if (GUILayout.Button("-"))
                {
                    itemToRemoveFromIndex = index;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (itemToAddAfterIndex != -1)
            {
                updateMultiCanvasGroupActionModel.CanvasGroups.Insert(itemToAddAfterIndex + 1, null);
            }
            else if (itemToRemoveFromIndex != -1 && updateMultiCanvasGroupActionModel.CanvasGroups.Count > 0)
            {
                updateMultiCanvasGroupActionModel.CanvasGroups.RemoveAt(itemToRemoveFromIndex);
            }

            updateMultiCanvasGroupActionModel.Alpha =
                EditorGUILayout.Slider("Alpha", updateMultiCanvasGroupActionModel.Alpha, 0.0f, 1.0f);
            updateMultiCanvasGroupActionModel.IsInteractible =
                EditorGUILayout.Toggle("Interactible", updateMultiCanvasGroupActionModel.IsInteractible);
            updateMultiCanvasGroupActionModel.BlockRaycasts =
                EditorGUILayout.Toggle("BlockRaycasts", updateMultiCanvasGroupActionModel.BlockRaycasts);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSetImageColorAction(SetImageColorActionModel setImageColor)
        {
            EditorGUILayout.BeginVertical();
            setImageColor.Image =
                (Image)EditorGUILayout.ObjectField("Image Component", setImageColor.Image, typeof(Image), true);
            setImageColor.Color = EditorGUILayout.ColorField(setImageColor.Color);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSetTextColorAction(SetTextColorActionModel setTextColor)
        {
            EditorGUILayout.BeginVertical();
            setTextColor.TextComponent =
                (TMP_Text)EditorGUILayout.ObjectField("Text Component", setTextColor.TextComponent, typeof(TMP_Text), true);
            setTextColor.Color = EditorGUILayout.ColorField(setTextColor.Color);
            EditorGUILayout.EndVertical();
        }


        private static void DrawSetTextAction(SetTextActionModel setTextModel)
        {
            EditorGUILayout.BeginVertical();
            setTextModel.TextComponent =
                (TMP_Text)EditorGUILayout.ObjectField("Text Component", setTextModel.TextComponent, typeof(TMP_Text), true);
            setTextModel.Text = EditorGUILayout.TextArea(setTextModel.Text);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSetGameCameraAction(SetGameCameraActionModel setGameCameraActionModel)
        {
            EditorGUIUtility.labelWidth = Screen.width * 0.2f;
            setGameCameraActionModel.CameraId = EditorGUILayout.TextField("CameraId", setGameCameraActionModel.CameraId);
            setGameCameraActionModel.IsActive =
                EditorGUILayout.Toggle("IsActive", setGameCameraActionModel.IsActive);
        }

        private static void DrawSetRectPropertyAction(SetRectPropertyActionModel rectPropertyActionModel)
        {
            EditorGUILayout.BeginVertical();
            if (rectPropertyActionModel.Targets == null)
            {
                rectPropertyActionModel.Targets = new List<RectTransform>();
            }

            var rList = rectPropertyActionModel.Targets;
            int rCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", rList.Count));
            while (rCount < rList.Count)
                rList.RemoveAt(rList.Count - 1);
            while (rCount > rList.Count)
                rList.Add(null);

            for (int i = 0; i < rList.Count; i++)
            {
                rList[i] = (RectTransform)EditorGUILayout.ObjectField("Rect", rList[i], typeof(RectTransform), true);
            }

            rectPropertyActionModel.TransformType =
                (PropertyType)EditorGUILayout.EnumPopup(rectPropertyActionModel.TransformType);
            rectPropertyActionModel.Value = EditorGUILayout.FloatField("Value:", rectPropertyActionModel.Value);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSetTransformPropertyAction(SetPropertyActionModel propertyActionModel)
        {
            EditorGUILayout.BeginVertical();
            if (propertyActionModel.Targets == null)
            {
                propertyActionModel.Targets = new List<Transform>();
            }

            var tList = propertyActionModel.Targets;
            int tCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", tList.Count));
            while (tCount < tList.Count)
                tList.RemoveAt(tList.Count - 1);
            while (tCount > tList.Count)
                tList.Add(null);

            for (int i = 0; i < tList.Count; i++)
            {
                tList[i] = (Transform)EditorGUILayout.ObjectField("Renderers", tList[i], typeof(Transform), true);
            }

            propertyActionModel.TransformType =
                (PropertyType)EditorGUILayout.EnumPopup(propertyActionModel.TransformType);
            propertyActionModel.Value = EditorGUILayout.FloatField("Value:", propertyActionModel.Value);
            EditorGUILayout.EndVertical();
        }

        private void DrawAnimateSharedAction(AnimateSharedColorActionModel animateSharedColorActionModel)
        {
            EditorGUILayout.BeginVertical();
            var sharedDropZone = DropZone("Drop elements here: ", 100, 50);
            if (animateSharedColorActionModel.Target == null)
            {
                animateSharedColorActionModel.Target = new List<Renderer>();
            }

            var sList = animateSharedColorActionModel.Target;
            int sNewCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", sList.Count));
            while (sNewCount < sList.Count)
                sList.RemoveAt(sList.Count - 1);
            while (sNewCount > sList.Count)
                sList.Add(null);

            if (sharedDropZone != null && sharedDropZone.Length > 0)
            {
                foreach (object o in sharedDropZone)
                {
                    GameObject go = (GameObject)o;
                    Renderer rnd = go.GetComponent<Renderer>();
                    if (rnd != null)
                        sList.Add(go.GetComponent<Renderer>());
                }
            }

            for (int i = 0; i < sList.Count; i++)
            {
                sList[i] = (Renderer)EditorGUILayout.ObjectField("Renderers", sList[i], typeof(Renderer), true);
            }

            animateSharedColorActionModel.TargetColor =
                (Color)EditorGUILayout.ColorField("Color:", animateSharedColorActionModel.TargetColor);
            animateSharedColorActionModel.MaterialId =
                EditorGUILayout.IntField("Material Id:", animateSharedColorActionModel.MaterialId);
            animateSharedColorActionModel.Duration =
                EditorGUILayout.FloatField("Duration:", animateSharedColorActionModel.Duration);
            EditorGUILayout.EndVertical();
        }

        private void DrawAnimationColorAction(AnimateColorActionModel animateColorActionModel)
        {
            EditorGUILayout.BeginVertical();
            var dropZone = DropZone("Drop elements here: ", 100, 50);
            if (animateColorActionModel.Target == null)
            {
                animateColorActionModel.Target = new List<Renderer>();
            }

            var list = animateColorActionModel.Target;
            int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            if (dropZone != null && dropZone.Length > 0)
            {
                foreach (object o in dropZone)
                {
                    GameObject go = (GameObject)o;
                    Renderer rnd = go.GetComponent<Renderer>();
                    if (rnd != null)
                        list.Add(go.GetComponent<Renderer>());
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = (Renderer)EditorGUILayout.ObjectField("Renderers", list[i], typeof(Renderer), true);
            }

            animateColorActionModel.TargetColor =
                (Color)EditorGUILayout.ColorField("Color:", animateColorActionModel.TargetColor);
            animateColorActionModel.MaterialId =
                EditorGUILayout.IntField("Material Id:", animateColorActionModel.MaterialId);
            animateColorActionModel.Duration = EditorGUILayout.FloatField("Duration:", animateColorActionModel.Duration);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSetTransformAction(SetTransformActionModel setTransformActionModel)
        {
            EditorGUILayout.BeginVertical();
            setTransformActionModel.IsLocal = EditorGUILayout.Toggle("Is Local", setTransformActionModel.IsLocal);
            EditorGUI.BeginChangeCheck();

            setTransformActionModel.Target =
                (GameObject)EditorGUILayout.ObjectField(setTransformActionModel.Target, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck())
            {
                // appluy values
                setTransformActionModel.Position = setTransformActionModel.Target.transform.localPosition;
                setTransformActionModel.Rotation = setTransformActionModel.Target.transform.localRotation.eulerAngles;
                setTransformActionModel.Scale = setTransformActionModel.Target.transform.localScale;
            }

            setTransformActionModel.Position =
                (Vector3)EditorGUILayout.Vector3Field("Position", setTransformActionModel.Position);
            setTransformActionModel.Rotation =
                (Vector3)EditorGUILayout.Vector3Field("Rotation", setTransformActionModel.Rotation);
            setTransformActionModel.Scale = (Vector3)EditorGUILayout.Vector3Field("Scale", setTransformActionModel.Scale);
            EditorGUILayout.EndVertical();
        }

        private void DrawSetActiveAction(SetActiveListActionModel setActiveListActionModel)
        {
            EditorGUILayout.BeginVertical();
            var gameObjectDropZone = DropZone("Drop elements here: ", 100, 50);
            if (setActiveListActionModel.Targets == null)
            {
                setActiveListActionModel.Targets = new List<GameObject>();
            }

            var gList = setActiveListActionModel.Targets;
            int gNewCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("GameObjects size", gList.Count));
            while (gNewCount < gList.Count)
                gList.RemoveAt(gList.Count - 1);
            while (gNewCount > gList.Count)
                gList.Add(null);

            if (gameObjectDropZone != null && gameObjectDropZone.Length > 0)
            {
                foreach (object o in gameObjectDropZone)
                {
                    GameObject go = (GameObject)o;
                    if (go != null)
                        gList.Add(go);
                }
            }

            for (int i = 0; i < gList.Count; i++)
            {
                gList[i] = (GameObject)EditorGUILayout.ObjectField("GameObjects", gList[i], typeof(GameObject), true);
            }

            EditorGUILayout.HelpBox("Please use this action with moderation !", MessageType.Warning);
            EditorGUILayout.EndVertical();
            setActiveListActionModel.State = EditorGUILayout.Toggle("Enabled", setActiveListActionModel.State);
        }

        private static void DrawSetActiveAction(SetActiveActionModel setActiveActionModel)
        {
            setActiveActionModel.Target =
                (GameObject)EditorGUILayout.ObjectField(setActiveActionModel.Target, typeof(GameObject), true);
            setActiveActionModel.State = EditorGUILayout.Toggle("Enabled", setActiveActionModel.State);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox("Please use this action with moderation !", MessageType.Warning);
            EditorGUILayout.EndVertical();
        }

        private void DrawSwapMaterialListAction(SwapMaterialListActionModel swapMaterialListActionModel)
        {
            EditorGUILayout.BeginVertical();
            var materialDropZone = DropZone("Drop elements here: ", 100, 50);
            if (swapMaterialListActionModel.Renderers == null)
            {
                swapMaterialListActionModel.Renderers = new List<Renderer>();
            }

            var mList = swapMaterialListActionModel.Renderers;
            int mNewCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", mList.Count));
            while (mNewCount < mList.Count)
                mList.RemoveAt(mList.Count - 1);
            while (mNewCount > mList.Count)
                mList.Add(null);

            if (materialDropZone != null && materialDropZone.Length > 0)
            {
                foreach (object o in materialDropZone)
                {
                    GameObject go = (GameObject)o;
                    Renderer rnd = go.GetComponent<Renderer>();
                    if (rnd != null)
                        mList.Add(go.GetComponent<Renderer>());
                }
            }

            for (int i = 0; i < mList.Count; i++)
            {
                mList[i] = (Renderer)EditorGUILayout.ObjectField("Renderers", mList[i], typeof(Renderer), true);
            }

            swapMaterialListActionModel.Index = EditorGUILayout.IntField("Index: ", swapMaterialListActionModel.Index);
            swapMaterialListActionModel.Material =
                (Material)EditorGUILayout.ObjectField(swapMaterialListActionModel.Material, typeof(Material), true);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSwapMaterialAction(SwapMaterialActionModel swapMaterialActionModel)
        {
            swapMaterialActionModel.Renderer =
                (Renderer)EditorGUILayout.ObjectField(swapMaterialActionModel.Renderer, typeof(Renderer), true);
            List<string> materialIndexes = new List<string> { "0" };
            if (swapMaterialActionModel.Renderer != null)
            {
                materialIndexes = new List<string>();
                for (int i = 0; i < swapMaterialActionModel.Renderer.sharedMaterials.Length; i++)
                {
                    materialIndexes.Add(i.ToString());
                }
            }

            swapMaterialActionModel.Index = EditorGUILayout.Popup(swapMaterialActionModel.Index, materialIndexes.ToArray());
            swapMaterialActionModel.Material =
                (Material)EditorGUILayout.ObjectField(swapMaterialActionModel.Material, typeof(Material), true);
        }

        private static void DrawUpdateColliderAction(ColliderActionModel colliderActionModel)
        {
            colliderActionModel.Collider =
                (Collider)EditorGUILayout.ObjectField(colliderActionModel.Collider, typeof(Collider), true);
            colliderActionModel.Enabled = EditorGUILayout.Toggle("Enabled", colliderActionModel.Enabled);
        }

        private static void DrawUpdateMeshRendererAction(MeshRendererActionModel meshRendererActionModel)
        {
            meshRendererActionModel.MeshRenderer =
                (MeshRenderer)EditorGUILayout.ObjectField(meshRendererActionModel.MeshRenderer, typeof(MeshRenderer), true);
            meshRendererActionModel.Enabled = EditorGUILayout.Toggle("Enabled", meshRendererActionModel.Enabled);
        }

        private static void DrawUpdateCanvasAction(UpdateCanvasGroupActionModel updateCanvasGroupActionModel)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            updateCanvasGroupActionModel.CanvasGroup =
                (CanvasGroup)EditorGUILayout.ObjectField(updateCanvasGroupActionModel.CanvasGroup, typeof(CanvasGroup),
                    true);
            updateCanvasGroupActionModel.Alpha =
                EditorGUILayout.Slider("Alpha", updateCanvasGroupActionModel.Alpha, 0.0f, 1.0f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            updateCanvasGroupActionModel.IsInteractible =
                EditorGUILayout.Toggle("Interactible", updateCanvasGroupActionModel.IsInteractible);
            updateCanvasGroupActionModel.BlockRaycasts =
                EditorGUILayout.Toggle("BlockRaycasts", updateCanvasGroupActionModel.BlockRaycasts);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private static void DrawSetSpriteAction(SetSpriteActionModel setSpriteActionModel)
        {
            setSpriteActionModel.Image =
                (Image)EditorGUILayout.ObjectField(setSpriteActionModel.Image, typeof(Image), true);
            setSpriteActionModel.Sprite =
                (Sprite)EditorGUILayout.ObjectField(setSpriteActionModel.Sprite, typeof(Sprite), true);
        }

        private void DrawStopAnimationAction(StopAnimationActionModel stopAnimationActionModel)
        {
            int stopClipIndex = string.IsNullOrEmpty(stopAnimationActionModel.Clip)
                ? 0
                : _animationClips.IndexOf(stopAnimationActionModel.Clip);
            stopClipIndex = EditorGUILayout.Popup(stopClipIndex, _animationClips.ToArray());
            stopClipIndex = Clamp(stopClipIndex, 0, _animationClips.Count);
            if (stopClipIndex < _animationClips.Count)
                stopAnimationActionModel.Clip = _animationClips[stopClipIndex];
        }

        public int Clamp(int val, int min, int max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        private void DrawAnimationAction(AnimationActionModel animationActionModel)
        {
            int clipIndex = string.IsNullOrEmpty(animationActionModel.Clip)
                ? 0
                : _animationClips.IndexOf(animationActionModel.Clip);
            clipIndex = EditorGUILayout.Popup(clipIndex, _animationClips.ToArray());
            clipIndex = Clamp(clipIndex, 0, _animationClips.Count);
            if (clipIndex < _animationClips.Count)
                animationActionModel.Clip = _animationClips[clipIndex];
            animationActionModel.Layer = EditorGUILayout.IntField("Layer", animationActionModel.Layer);
            animationActionModel.Weight = EditorGUILayout.Slider(animationActionModel.Weight, 0.0f, 1.0f);
        }

        private void DrawSetColliderAction(SetColliderActiveActionModel setColliderActiveActionModel)
        {
            setColliderActiveActionModel.Collider =
                (Collider)EditorGUILayout.ObjectField(setColliderActiveActionModel.Collider, typeof(Collider), true);
            setColliderActiveActionModel.Enabled = EditorGUILayout.Toggle("Enabled", setColliderActiveActionModel.Enabled);
        }

        private void DrawDoRotateAction(DoRotateActionModel doRotateActionModel)
        {
            EditorGUILayout.BeginVertical();
            doRotateActionModel.Target = (Transform)EditorGUILayout.ObjectField("Target",
                doRotateActionModel.Target, typeof(Transform), true);
            doRotateActionModel.Axis = EditorGUILayout.Vector3Field("Axis: ", doRotateActionModel.Axis);
            doRotateActionModel.Duration = EditorGUILayout.FloatField("Duration :", doRotateActionModel.Duration);
            doRotateActionModel.Loop = EditorGUILayout.Toggle("Loop :", doRotateActionModel.Loop);
            EditorGUILayout.EndVertical();
        }

        private void DrawDoShakePositionAction(DoShakePositionActionModel doShakePositionActionModel)
        {
            EditorGUILayout.BeginVertical();
            doShakePositionActionModel.Target = (Transform)EditorGUILayout.ObjectField("Target",
                doShakePositionActionModel.Target, typeof(Transform), true);
            doShakePositionActionModel.Loop = EditorGUILayout.Toggle("Loop :", doShakePositionActionModel.Loop);
            doShakePositionActionModel.Duration =
                EditorGUILayout.FloatField("Duration :", doShakePositionActionModel.Duration);
            doShakePositionActionModel.Strength =
                EditorGUILayout.Vector3Field("Strength: ", doShakePositionActionModel.Strength);
            doShakePositionActionModel.Vibaration =
                EditorGUILayout.IntField("Vibration: ", doShakePositionActionModel.Vibaration);
            doShakePositionActionModel.Randomness =
                EditorGUILayout.FloatField("Randomness :", doShakePositionActionModel.Randomness);
            doShakePositionActionModel.FadeOut = EditorGUILayout.Toggle("FadeOut :", doShakePositionActionModel.FadeOut);
            EditorGUILayout.EndVertical();
        }

        private void DrawDoPunchScaleAction(DoPunchScaleActionModel doPunchScaleActionModel)
        {
            EditorGUILayout.BeginVertical();
            doPunchScaleActionModel.Target = (Transform)EditorGUILayout.ObjectField("Target",
                doPunchScaleActionModel.Target, typeof(Transform), true);
            doPunchScaleActionModel.Punch = EditorGUILayout.Vector3Field("Punch: ", doPunchScaleActionModel.Punch);
            doPunchScaleActionModel.Duration = EditorGUILayout.FloatField("Duration :", doPunchScaleActionModel.Duration);
            doPunchScaleActionModel.Vibaration =
                EditorGUILayout.IntField("Vibration: ", doPunchScaleActionModel.Vibaration);
            doPunchScaleActionModel.Elasticity =
                EditorGUILayout.FloatField("Elasticity :", doPunchScaleActionModel.Elasticity);
            EditorGUILayout.EndVertical();
        }

        public int GetEaseValueSelected()
        {
            int selected = EditorGUILayout.Popup("DropDown", 0, Enum.GetNames(typeof(Ease)).ToArray());
            return selected;
        }

        private void DrawDoMoveAction(DoMoveActionModel doMoveActionModel)
        {
            EditorGUILayout.BeginVertical();
            doMoveActionModel.Target = (Transform)EditorGUILayout.ObjectField("Target",
                doMoveActionModel.Target, typeof(Transform), true);
            doMoveActionModel.MoveTo = EditorGUILayout.Vector3Field("Move To: ", doMoveActionModel.MoveTo);
            doMoveActionModel.Duration = EditorGUILayout.FloatField("Duration :", doMoveActionModel.Duration);
            doMoveActionModel.Snapping = EditorGUILayout.Toggle("Snapping: ", doMoveActionModel.Snapping);
            doMoveActionModel.EaseType = (Ease)EditorGUILayout.EnumPopup("Ease: ", doMoveActionModel.EaseType);
            //Debug.Log(ease);
            EditorGUILayout.EndVertical();

        }

        private void DrawDoFadeAction(DoFadeActionModel doFadeActionModel)
        {
            EditorGUILayout.BeginVertical();
            doFadeActionModel.EaseType = (Ease)EditorGUILayout.EnumPopup("Ease Type", doFadeActionModel.EaseType);
            doFadeActionModel.Target = (CanvasGroup)EditorGUILayout.ObjectField("Target",
                doFadeActionModel.Target, typeof(CanvasGroup), true);
            doFadeActionModel.FadeTo = EditorGUILayout.FloatField("Fade To: ", doFadeActionModel.FadeTo);
            doFadeActionModel.Duration = EditorGUILayout.FloatField("Duration :", doFadeActionModel.Duration);
            EditorGUILayout.BeginHorizontal();
            doFadeActionModel.IsInteractible =
                EditorGUILayout.Toggle("Interactible", doFadeActionModel.IsInteractible);
            doFadeActionModel.BlockRaycasts =
                EditorGUILayout.Toggle("BlockRaycasts", doFadeActionModel.BlockRaycasts);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawDoFadeGroupAction(DoFadeGroupActionModel doFadeActionModel)
        {
            EditorGUILayout.BeginVertical();
            var dropZone = DropZone("Drag Canvas Groups Here", 100, 50);
            if (doFadeActionModel.Targets == null)
            {
                doFadeActionModel.Targets = new List<CanvasGroup>();
            }

            var cList = doFadeActionModel.Targets;
            int cNewcount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", cList.Count));
            while (cNewcount < cList.Count)
                cList.RemoveAt(cList.Count - 1);
            while (cNewcount > cList.Count)
                cList.Add(null);

            if (dropZone != null && dropZone.Length > 0)
            {
                foreach (object o in dropZone)
                {
                    GameObject go = (GameObject)o;
                    CanvasGroup cnvGrp = go.GetComponent<CanvasGroup>();
                    if (cnvGrp != null)
                        cList.Add(cnvGrp);
                }
            }

            for (int i = 0; i < cList.Count; i++)
            {
                cList[i] = (CanvasGroup)EditorGUILayout.ObjectField("Canvas Group", cList[i], typeof(CanvasGroup), true);
            }


            doFadeActionModel.EaseType = (Ease)EditorGUILayout.EnumPopup("Ease Type", doFadeActionModel.EaseType);
            EditorGUILayout.BeginHorizontal();
            doFadeActionModel.FadeTo = EditorGUILayout.FloatField("Fade To: ", doFadeActionModel.FadeTo);
            doFadeActionModel.Duration = EditorGUILayout.FloatField("Duration :", doFadeActionModel.Duration);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            doFadeActionModel.IsInteractible =
                EditorGUILayout.Toggle("Interactible", doFadeActionModel.IsInteractible);
            doFadeActionModel.BlockRaycasts =
                EditorGUILayout.Toggle("BlockRaycasts", doFadeActionModel.BlockRaycasts);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawSetMaterialAction(SetMaterialActionModel setMaterialActionModel)
        {
            EditorGUILayout.BeginVertical();
            setMaterialActionModel.Image =
                (Image)EditorGUILayout.ObjectField(setMaterialActionModel.Image, typeof(Image), true);
            setMaterialActionModel.Material =
                (Material)EditorGUILayout.ObjectField(setMaterialActionModel.Material, typeof(Material), true);
            EditorGUILayout.EndVertical();
        }

        private void DrawContextMenu(Rect stateRect, SetRectTransformByNameActionModel setRectTransformByNameActionModel,
            GenericMenu menu)
        {
            if (Event.current.type == EventType.ContextClick)
            {
                Vector2 mousePos = Event.current.mousePosition;
                if (stateRect.Contains(mousePos))
                {
                    GameObject temp = new GameObject("_TEMP");
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(temp))
                    {
                        RectTransform rectTransform = temp.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            if (menu == null)
                            {
                                menu = new GenericMenu();
                            }

                            menu.AddItem(new GUIContent("Paste Rect Transform Values"), false, () =>
                            {
                                GameObject temp2 = new GameObject("_TEMP");
                                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(temp2);
                                RectTransform rectTransform2 = temp2.GetComponent<RectTransform>();
                                setRectTransformByNameActionModel.Position = rectTransform2.anchoredPosition3D;
                                setRectTransformByNameActionModel.WH = rectTransform2.sizeDelta;
                                setRectTransformByNameActionModel.AnchorMin = rectTransform2.anchorMin;
                                setRectTransformByNameActionModel.AnchorMax = rectTransform2.anchorMax;
                                setRectTransformByNameActionModel.Pivot = rectTransform2.pivot;
                                setRectTransformByNameActionModel.Rotation = rectTransform2.rotation.eulerAngles;
                                setRectTransformByNameActionModel.Scale = rectTransform2.localScale;
                                DestroyImmediate(temp2);
                            });
                        }
                    }

                    DestroyImmediate(temp);
                }
            }
        }

        private void DrawContextMenu(Rect stateRect, SetRectTransformActionModel setRectTransformActionModel,
            GenericMenu menu)
        {
            if (Event.current.type == EventType.ContextClick)
            {
                Vector2 mousePos = Event.current.mousePosition;
                if (stateRect.Contains(mousePos))
                {
                    GameObject temp = new GameObject("_TEMP");
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(temp))
                    {
                        RectTransform rectTransform = temp.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            if (menu == null)
                            {
                                menu = new GenericMenu();
                            }

                            menu.AddItem(new GUIContent("Paste Rect Transform Values"), false, () =>
                            {
                                GameObject temp2 = new GameObject("_TEMP");
                                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(temp2);
                                RectTransform rectTransform2 = temp2.GetComponent<RectTransform>();
                                setRectTransformActionModel.Position = rectTransform2.anchoredPosition3D;
                                setRectTransformActionModel.WH = rectTransform2.sizeDelta;
                                setRectTransformActionModel.AnchorMin = rectTransform2.anchorMin;
                                setRectTransformActionModel.AnchorMax = rectTransform2.anchorMax;
                                setRectTransformActionModel.Pivot = rectTransform2.pivot;
                                setRectTransformActionModel.Rotation = rectTransform2.rotation.eulerAngles;
                                setRectTransformActionModel.Scale = rectTransform2.localScale;
                                DestroyImmediate(temp2);
                            });
                        }
                    }

                    DestroyImmediate(temp);
                }
            }
        }

        private void DrawContextMenu(Rect stateRect, SetTransformByNameActionModel setTransformByNameActionModel,
            GenericMenu menu)
        {
            if (Event.current.type == EventType.ContextClick)
            {
                Vector2 mousePos = Event.current.mousePosition;
                if (stateRect.Contains(mousePos))
                {
                    GameObject temp = new GameObject("_TEMP");
                    if (UnityEditorInternal.ComponentUtility.PasteComponentValues(temp.transform))
                    {
                        if (menu == null)
                        {
                            menu = new GenericMenu();
                        }

                        menu.AddItem(new GUIContent("Paste Transform Values"), false, () =>
                        {
                            GameObject temp3 = new GameObject("_TEMP");
                            UnityEditorInternal.ComponentUtility.PasteComponentValues(temp3.transform);
                            setTransformByNameActionModel.Position = temp3.transform.position;
                            setTransformByNameActionModel.Rotation = temp3.transform.rotation.eulerAngles;
                            setTransformByNameActionModel.Scale = temp3.transform.localScale;
                            DestroyImmediate(temp3);
                        });
                    }

                    DestroyImmediate(temp);
                }
            }
        }

        private void DrawSetGraphicsRaycasters(SetGraphicsRaycasterActionModel setGraphicsRaycasterActionModel)
        {
            {
                if (setGraphicsRaycasterActionModel.Raycasters == null ||
                    setGraphicsRaycasterActionModel.Raycasters.Count <= 0)
                    setGraphicsRaycasterActionModel.Raycasters = new List<GraphicRaycaster>() { null };
            }
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("F|All"))
            {
                setGraphicsRaycasterActionModel.Raycasters = ((SimpleStateComponent)target)
                    .GetComponentsInChildren<GraphicRaycaster>().ToList();
            }

            if (GUILayout.Button("F|W|Canvas Only"))
            {
                Canvas[] canvases = ((SimpleStateComponent)target).GetComponentsInChildren<Canvas>();
                if (canvases.Length > 0)
                {
                    List<GraphicRaycaster> raycastersGroups = new List<GraphicRaycaster>();
                    foreach (Canvas canvase in canvases)
                    {
                        GraphicRaycaster raycaster = canvase.GetComponent<GraphicRaycaster>();
                        if (raycaster != null)
                        {
                            raycastersGroups.Add(raycaster);
                        }
                    }

                    if (raycastersGroups.Count > 0)
                        setGraphicsRaycasterActionModel.Raycasters = raycastersGroups;
                    else
                        setGraphicsRaycasterActionModel.Raycasters = new List<GraphicRaycaster>() { null };
                }
            }

            if (GUILayout.Button("Clear All"))
            {
                setGraphicsRaycasterActionModel.Raycasters = new List<GraphicRaycaster>() { null };
            }

            EditorGUILayout.EndHorizontal();
            int itemToAddAfterIndex = -1;
            int itemToRemoveFromIndex = -1;
            for (var index = 0; index < setGraphicsRaycasterActionModel.Raycasters.Count; index++)
            {
                GraphicRaycaster raycaster = setGraphicsRaycasterActionModel.Raycasters[index];
                EditorGUILayout.BeginHorizontal();
                raycaster = (GraphicRaycaster)EditorGUILayout.ObjectField(raycaster, typeof(GraphicRaycaster), true);
                if (GUILayout.Button("+"))
                {
                    itemToAddAfterIndex = index;
                }

                if (GUILayout.Button("-"))
                {
                    itemToRemoveFromIndex = index;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (itemToAddAfterIndex != -1)
            {
                setGraphicsRaycasterActionModel.Raycasters.Insert(itemToAddAfterIndex + 1, null);
            }
            else if (itemToRemoveFromIndex != -1 && setGraphicsRaycasterActionModel.Raycasters.Count > 0)
            {
                setGraphicsRaycasterActionModel.Raycasters.RemoveAt(itemToRemoveFromIndex);
            }

            setGraphicsRaycasterActionModel.IsEnabled =
                EditorGUILayout.Toggle("Enabled: ", setGraphicsRaycasterActionModel.IsEnabled);
            EditorGUILayout.EndVertical();
        }

        public object[] DropZone(string title, int w, int h)
        {
            Rect myRect = GUILayoutUtility.GetRect(w, h, GUILayout.ExpandWidth(true));
            GUI.Box(myRect, "Drag and Drop GameObjects to this Box!");
            if (myRect.Contains(Event.current.mousePosition))
            {
                EventType eventType = Event.current.type;
                bool isAccepted = false;

                if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        isAccepted = true;
                    }

                    Event.current.Use();
                }

                return isAccepted ? DragAndDrop.objectReferences : null;
            }

            return null;
        }
    }
}
#endif
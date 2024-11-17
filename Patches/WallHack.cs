﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnifromEngine.Patches
{
    public class WallHack : MonoBehaviour
    {
        public GrabbableObject[] grabbableObjects;
        public EntranceTeleport[] exits;

        public List<GameObject> ExitDoorTextObjects = new List<GameObject>();
        public List<GameObject> ItemTextObjects = new List<GameObject>();
        public List<GameObject> EnemyTextObjects = new List<GameObject>();
        public List<GameObject> PlayerTextObjects = new List<GameObject>();

        public void RenderItems()
        {
            if (!Engine.Instance.isWallHackOn)
            {
                ClearItemExistingLabels();
                ClearEnemyExistingLabels();
                ClearPlayerExistingLabels();
                return;
            }

            if (Engine.Instance.isItemWallHackOn)
            {
                if (grabbableObjects == null || grabbableObjects.Length == 0)
                    return;

                foreach (var grabbable in grabbableObjects)
                {
                    if (grabbable == null)
                        continue;

                    TextMesh itemTextMesh = grabbable.GetComponentInChildren<TextMesh>();

                    if (grabbable.parentObject != null)
                    {
                        if (itemTextMesh != null)
                            itemTextMesh.gameObject.SetActive(false);

                        continue;
                    }

                    if (itemTextMesh == null)
                    {
                        GameObject itemLabel = new GameObject("GrabbableLabel");
                        ItemTextObjects.Add(itemLabel);

                        itemLabel.transform.SetParent(grabbable.transform);
                        itemLabel.transform.localPosition = Vector3.zero;

                        itemTextMesh = itemLabel.AddComponent<TextMesh>();
                        itemTextMesh.text = "☐";
                        itemTextMesh.characterSize = 0.3f;
                        itemTextMesh.color = new Color32(255, 150, 0, 255);
                        itemTextMesh.anchor = TextAnchor.MiddleCenter;
                        itemTextMesh.transform.LookAt(RoundManager.Instance.playersManager.activeCamera.transform);
                        itemTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                    else
                    {
                        itemTextMesh.transform.LookAt(RoundManager.Instance.playersManager.activeCamera.transform);
                        itemTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }

                    if (Engine.Instance.isItemWallHackOn)
                    {
                        if (Engine.Instance.sortByPrice && Engine.Instance.sortPrice > grabbable.scrapValue)
                            itemTextMesh.text = null;
                        else
                            if (Engine.Instance.hideBigItems && grabbable.itemProperties.twoHanded)
                                itemTextMesh.text = null;
                            else
                                if (Engine.Instance.showItemPrice)
                                    if (Engine.Instance.sortSize <= 0)
                                        itemTextMesh.text = $"\n    ☐\n<color=#ff0064>Price: {grabbable.scrapValue}</color>";
                                    else
                                        if (Engine.Instance.sortSize > 18)
                                            itemTextMesh.text = $"<color=#ff0064><size={Engine.Instance.sortSize}>[Price: {grabbable.scrapValue}]</size></color>";
                                        else
                                            itemTextMesh.text = $"\n    ☐\n<color=#ff0064><size={Engine.Instance.sortSize}>Price: {grabbable.scrapValue}</size></color>";
                                else
                                    itemTextMesh.text = "☐";
                    }
                }
            }
            else
                ClearItemExistingLabels();
        }

        public void RenderEnemies()
        {
            if (!Engine.Instance.isWallHackOn)
                return;

            if (Engine.Instance.isEnemyWallHackOn)
            {
                if (SpawnedEnemies() == null || SpawnedEnemies().Count == 0)
                    return;

                foreach (var enemy in SpawnedEnemies())
                {
                    if (enemy == null)
                        continue;

                    TextMesh enemyTextMesh = enemy.GetComponentInChildren<TextMesh>();

                    if (enemyTextMesh == null)
                    {
                        GameObject enemyLabel = new GameObject("EnemyLabel");
                        EnemyTextObjects.Add(enemyLabel);

                        enemyLabel.transform.SetParent(enemy.transform);
                        enemyLabel.transform.localPosition = Vector3.zero;

                        enemyTextMesh = enemyLabel.AddComponent<TextMesh>();
                        enemyTextMesh.text = $"             ☐ \n {enemy.name}";
                        enemyTextMesh.characterSize = 0.5f;
                        enemyTextMesh.color = Color.red;
                        enemyTextMesh.anchor = TextAnchor.MiddleCenter;
                        enemyTextMesh.transform.LookAt(GetPlayer().activeCamera.transform);
                        enemyTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                    else
                    {
                        enemyTextMesh.transform.LookAt(GetPlayer().activeCamera.transform);
                        enemyTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                }
            }
            else
                ClearEnemyExistingLabels();
        }

        public void RenderPlayers()
        {
            if (!Engine.Instance.isWallHackOn)
                return;
            
            if (Engine.Instance.isPlayerWallHackOn)
            {
                foreach (var player in RoundManager.Instance.playersManager.allPlayerObjects)
                {
                    if (player == null || RoundManager.Instance.playersManager.localPlayerController.name == player.name)
                        continue;

                    TextMesh playerTextMesh = player.GetComponentInChildren<TextMesh>();

                    if (playerTextMesh == null)
                    {
                        GameObject playerLabel = new GameObject("PlayerLabel");
                        PlayerTextObjects.Add(playerLabel);

                        playerLabel.transform.SetParent(player.transform);
                        playerLabel.transform.localPosition = Vector3.zero;

                        playerTextMesh = playerLabel.AddComponent<TextMesh>();
                        playerTextMesh.text = $"    ☐ \n {player.name}";
                        playerTextMesh.characterSize = 0.5f;
                        playerTextMesh.color = Color.cyan;
                        playerTextMesh.anchor = TextAnchor.MiddleCenter;
                        playerTextMesh.transform.LookAt(GetPlayer().activeCamera.transform); 
                        playerTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                    else
                    {
                        playerTextMesh.transform.LookAt(GetPlayer().activeCamera.transform);
                        playerTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                }
            }
            else
                ClearPlayerExistingLabels();
        }

        public void RenderDoors()
        {
            if (Engine.Instance.isWallHackOn && Engine.Instance.showExits)
            {
                foreach (var exit in exits)
                {
                    TextMesh exitDoorTextMesh = exit.GetComponentInChildren<TextMesh>();

                    if (exitDoorTextMesh == null)
                    {
                        GameObject exitDoorLabel = new GameObject("exitDoorLabel");
                        ExitDoorTextObjects.Add(exitDoorLabel);

                        exitDoorLabel.transform.SetParent(exit.gameObject.transform);
                        exitDoorLabel.transform.localPosition = Vector3.zero;

                        exitDoorTextMesh = exitDoorLabel.AddComponent<TextMesh>();
                        exitDoorTextMesh.characterSize = 0.8f;
                        exitDoorTextMesh.transform.localScale = Vector3.one;

                        if (exit.name == "EntranceTeleportA" || exit.name == "EntranceTeleportA(Clone)")
                        {
                            exitDoorTextMesh.text = "▮▮";
                            exitDoorTextMesh.color = new Color32(100, 120, 255, 255);
                        }
                        else
                        {
                            exitDoorTextMesh.text = "▮";
                            exitDoorTextMesh.color = new Color32(255, 50, 0, 255);
                        }

                        exitDoorTextMesh.anchor = TextAnchor.MiddleCenter;
                        exitDoorTextMesh.transform.LookAt(GetPlayer().activeCamera.transform);
                        exitDoorTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                    else
                    {
                        exitDoorTextMesh.transform.LookAt(GetPlayer().activeCamera.transform);
                        exitDoorTextMesh.transform.rotation = Quaternion.LookRotation(GetPlayer().activeCamera.transform.forward);
                    }
                }
            }
            else
                ClearExitDoorExistingLabels();

        }
        

        private void ClearItemExistingLabels()
        {
            foreach (var textObject in ItemTextObjects)
            {
                Destroy(textObject);
            }
            
            ItemTextObjects.Clear();
        }
        
        private void ClearEnemyExistingLabels()
        {
            foreach (var textObject in EnemyTextObjects)
            {
                Destroy(textObject);
            }
            
            EnemyTextObjects.Clear();
        }
        
        private void ClearPlayerExistingLabels()
        {
            foreach (var textObject in PlayerTextObjects)
            {
                Destroy(textObject);
            }
            
            PlayerTextObjects.Clear();
        }

        private void ClearExitDoorExistingLabels()
        {
            foreach (var exit in ExitDoorTextObjects)
            {
                Destroy(exit);
            }
            
            EnemyTextObjects.Clear();
        }
        
        
        private IEnumerator SearchItems()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                grabbableObjects = FindObjectsOfType<GrabbableObject>();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator SearchEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                SpawnedEnemies();
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        private IEnumerator SearchExits()
        {
            yield return new WaitForSeconds(5f);
            exits = FindObjectsOfType<EntranceTeleport>();
        }

        
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            StopCoroutine(SearchItems());
            StartCoroutine(SearchItems());

            StopCoroutine(SearchEnemies());
            StartCoroutine(SearchEnemies());
            
            StopCoroutine(SearchExits());
            StartCoroutine(SearchExits());
        }

        private static StartOfRound GetPlayer()
        {
            return RoundManager.Instance.playersManager;
        }
        
        private static List<EnemyAI> SpawnedEnemies()
        {
            return RoundManager.Instance.SpawnedEnemies;
        }
    }
}
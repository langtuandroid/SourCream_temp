using UnityEngine;
using System;
using System.Collections.Generic;

public class CooldownManager : MonoBehaviour
{
    private class Cooldown
    {
        public string identifier;
        public float duration;
        public float currentCooldown;
        public bool isCooldownRunning;
        public Action onCooldownFinished;
    }

    private HashSet<string> cooldownIdentifiers = new HashSet<string>();
    private List<Cooldown> cooldowns = new List<Cooldown>();

    // Call this method to start a new cooldown with the specified identifier, duration, and callback
    public void StartCooldown(string identifier, float duration, Action onCooldownFinishedCallback = null)
    {
        // Check if the identifier is not already in the list
        if (!cooldownIdentifiers.Contains(identifier)) {
            cooldownIdentifiers.Add(identifier);
            Cooldown newCooldown = new Cooldown {
                identifier = identifier,
                duration = duration,
                currentCooldown = duration,
                isCooldownRunning = true,
                onCooldownFinished = onCooldownFinishedCallback
            };
            cooldowns.Add(newCooldown);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = cooldowns.Count - 1; i >= 0; i--) {
            Cooldown cooldown = cooldowns[i];

            if (cooldown.isCooldownRunning) {
                cooldown.currentCooldown -= Time.deltaTime;
                if (cooldown.currentCooldown <= 0) {
                    cooldown.currentCooldown = 0;
                    cooldown.isCooldownRunning = false;
                    cooldown.onCooldownFinished?.Invoke();
                    cooldownIdentifiers.Remove(cooldown.identifier);
                    cooldowns.RemoveAt(i);
                }
            }
        }
    }

    // Call this method to reduce the remaining cooldown time of a specific cooldown
    public void ReduceCooldown(float amount, string identifier)
    {
        for (int i = 0; i < cooldowns.Count; i++) {
            if (cooldowns[i].identifier == identifier) {
                Cooldown cooldown = cooldowns[i];
                cooldown.currentCooldown -= amount;
                if (cooldown.currentCooldown <= 0) {
                    cooldown.currentCooldown = 0;
                    cooldown.isCooldownRunning = false;
                    cooldown.onCooldownFinished?.Invoke();
                    cooldownIdentifiers.Remove(cooldown.identifier);
                    cooldowns.RemoveAt(i);
                }
                break;
            }
        }
    }

    // Call this method to check if a specific cooldown is still ongoing
    public bool IsCooldownRunning(string identifier)
    {
        foreach (Cooldown cooldown in cooldowns) {
            if (cooldown.identifier == identifier) {
                return cooldown.isCooldownRunning;
            }
        }
        return false;
    }

    // Call this method to get the remaining time of a specific cooldown
    public float GetRemainingTime(string identifier)
    {
        foreach (Cooldown cooldown in cooldowns) {
            if (cooldown.identifier == identifier) {
                return cooldown.currentCooldown;
            }
        }
        return 0f;
    }
}

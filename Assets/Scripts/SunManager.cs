﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    public static SunManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public DragSun dragSun;
    public DragSun dragMoon;
    DragSun dragAstro;
    public SpriteRenderer background;

    public BezierPath sunPath;
    public float environmentTemperature = 15f; //De -10 a 40 grados centígrados
    public float tempFactor;
    public float tempXHit = 0.1f;
    public int numOfCloudsToBlockSun = 15;

    public Color dayBG, nightBG;//new Color32(255, 255, 255, 255) : new Color32(0, 35, 128, 255);

    public void UpdateTempFactor()
    {
        float dayOrNight = SeedStateManager.Instance.isDay ? 1 : -1;
        dragAstro = SeedStateManager.Instance.isDay ? dragSun : dragMoon;
        background.color = SeedStateManager.Instance.isDay ? dayBG : nightBG;

        if ((float)dragAstro.position / (float)sunPath.numOfPoints < 0.05f || (float)dragAstro.position / (float)sunPath.numOfPoints > 0.95f)
        {
            tempFactor = 0f;
        }
        else if ((float)dragAstro.position / (float)sunPath.numOfPoints < 0.5f)
        {
            tempFactor = dayOrNight * Mathf.Lerp(0.1f, 1, (float)dragAstro.position / ((float)sunPath.numOfPoints / 2));
        }
        else
        {
            tempFactor = dayOrNight * Mathf.Lerp(1, 0.1f, (((float)dragAstro.position / ((float)sunPath.numOfPoints / 2)) - 1));
        }

        tempFactor *= SeedStateManager.Instance.isDay ?
            1f - (float)WaterManager.Instance.cloudsAlive.Count / (float)numOfCloudsToBlockSun :
            1f + (float)WaterManager.Instance.cloudsAlive.Count / (float)numOfCloudsToBlockSun; // OJO CUIDAO QUE EL CLOUDFACTOR ESTA MAL POR AHORA
    }

    bool temperatureOff = false;
    public void UpdateTempStat(Threshold tempTH, Threshold waterTH, Threshold airTH)
    {
        SeedStateManager ssm = SeedStateManager.Instance;
        environmentTemperature += tempXHit * tempFactor * Time.deltaTime;
        ssm.stats.temperature = Mathf.Lerp(ssm.stats.temperature, environmentTemperature, ssm.stats.temperatureReception / 100);

        float actualUpTh = tempTH.up, actualDownTh = tempTH.down;
        if (ssm.stats.airLevel > airTH.up) // CAMBIOS SEGUN OTROS PARAMETROS COMO VIENTO O AGUA
        {
            actualDownTh += airTH.outsideThChanger;
        }
        else if (ssm.stats.airLevel < airTH.down)
        {
            actualUpTh -= airTH.outsideThChanger;
        }

        if (!temperatureOff)
        {
            if (ssm.stats.temperature >= actualUpTh)
            {
                ssm.stats.StateChange(PlantState.DRY, true);
                temperatureOff = true;
            }
            else if (ssm.stats.temperature <= actualDownTh)
            {
                ssm.stats.StateChange(PlantState.FROZEN, true);
                temperatureOff = true;
            }
        }
        else if (ssm.stats.temperature < actualUpTh && ssm.stats.temperature > actualDownTh)
        {
            temperatureOff = false;
            ssm.stats.StateChange(PlantState.DRY, false);
            ssm.stats.StateChange(PlantState.FROZEN, false);
        }
    }
}

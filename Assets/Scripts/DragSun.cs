﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSun : MonoBehaviour
{
    private Vector3 mOffset;
    public BezierPath path;
    public int position = 0;
    public float secsToAdvance = 1f;

    float timer;

    private void Start()
    {
        transform.position = path.positions[position];
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > secsToAdvance)
        {
            SunAdvance();
            timer = 0;
        }
    }
    private void SunAdvance()
    {
        position++;
        if (position >= path.positions.Length)
        {
            position = 0;
        }
        transform.position = path.positions[position];
        SunManager.Instance.UpdateTempFactor();
    }
    private void OnMouseDown()
    {
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        transform.position = FindClosestPoint();
        SunManager.Instance.UpdateTempFactor();
        timer = 0;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    private Vector3 FindClosestPoint()
    {
        Vector3 closest = path.positions[path.numOfPoints-1];
        float lastDistance = 9999f;
        for (int i = 0; i < path.positions.Length; i++)
        {
            Vector3 item = (Vector3)path.positions[i];
            if (Vector3.Distance(GetMouseWorldPos(), item) < lastDistance)
            {
                closest = item;
                lastDistance = Vector3.Distance(GetMouseWorldPos(), item);
                position = i;
            }
        }
        
        return closest;
    }
}

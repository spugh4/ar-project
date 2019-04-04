﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using System;

public class PetController : MonoBehaviour
{
    public GameObject pet;
    public GameObject placementIndicator;
    public GameObject petPlane;
    public GameObject[] foods;

    public float correctAnswer;

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool validPlacementPose = false;
    private bool placed = false;
    Vector3 petPlanePos;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        pet.SetActive(false);
        petPlane.SetActive(false);

        ShowFood(false);
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(validPlacementPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!placed) {
                PlacePet();
                PlaceFood();
                ShowFood(true);
                correctAnswer = 7;
            }
            else 
            {
                try
                {
                    pet.GetComponent<CatMoveTo>().StartMove(placementPose.position);
                } catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                }
            }

        }

    }

    void UpdatePlacementIndicator()
    {
        Vector3 placeIndPos = new Vector3(placementPose.position.x, placementPose.position.y + .001f, placementPose.position.z);

        if(!placed)
        {
            if (validPlacementPose)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placeIndPos, placementPose.rotation);
            }
            else
            {
                placementIndicator.SetActive(false);
            }
        } else
        {
            if(validPlacementPose)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placeIndPos, placementPose.rotation);
            } else
            {
                placementIndicator.SetActive(false);
            }
        }
    }

    void UpdatePlacementPose()
    {
        Vector3 screencenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var cameraForward = Camera.current.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        if (!placed)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            arOrigin.Raycast(screencenter, hits, TrackableType.Planes);

            validPlacementPose = hits.Count > 0;

            if (validPlacementPose)
            {
                placementPose = hits[0].pose;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                if(hit.collider.gameObject.name == "Pet Plane")
                {
                    validPlacementPose = true;
                    placementPose.position = hit.point;
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                }
            }
            else
            {
                validPlacementPose = false;
            }
        }
    }

    void PlacePet()
    {
        placed = true;

        Vector3 placePosePos = placementPose.position;

        petPlanePos = new Vector3(placePosePos.x, placePosePos.y, placePosePos.z);

        petPlane = Instantiate(petPlane, placePosePos, Quaternion.identity);
        pet = Instantiate(pet, placePosePos, placementPose.rotation);
        petPlane.name = "Pet Plane";

        pet.SetActive(true);
        petPlane.SetActive(true);
    }

    void PlaceFood()
    {
        Vector3 back = new Vector3(0, 0.1f, 0.35f);
        Vector3 right = new Vector3(0.35f, 0.2f, 0);
        Vector3 left = new Vector3(-0.35f, 0.15f, 0);

        Vector3 catPos = placementPose.position;

        GameObject cherry = foods[0];
        GameObject cake = foods[1];
        GameObject hamburger = foods[2];

        cherry.GetComponent<FoodScript>().setValue(6.5f);
        cake.GetComponent<FoodScript>().setValue(8f);
        hamburger.GetComponent<FoodScript>().setValue(7f);

        cherry.transform.position = new Vector3(catPos.x, catPos.y + .1f, catPos.z + .4f);
        cake.transform.position = new Vector3(catPos.x, catPos.y + .1f, catPos.z - .4f);
        hamburger.transform.position = new Vector3(catPos.x + .4f, catPos.y + .05f, catPos.z);
    }

    void ShowFood(bool show)
    {
        foreach(GameObject food in foods)
        {
            if(show)
            {
                food.SetActive(true);
            } else
            {
                food.SetActive(false);
            }
        }
    }
}

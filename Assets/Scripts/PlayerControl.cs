using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    float xAxis, yAxis;
    bool runButton;
    [Range(1f, 20f)]
    public float defaultWalkSpeed = 2.7f;
    [Range(1f, 20f)]
    public float runSpeedFactor = 2;
    float playerSpeed = 2.9f;
    [Range(0f,1f)]
    public float smoothing = 0.1f;

    [Header("Item Prefabs")]
    public GameObject SmokeBombPrefab;
    public GameObject GluePrefab;
    public GameObject StonePrefab;

    // Glue variablen
    float glueDuration;
    public float glueLifeDuration = 10f;
    public float glueSpeedFactor = 0.5f;
    bool isGlued = false;

    float cloakDuration;


    Rigidbody rigidBody;
    Vector3 lastPosition;
    Vector3 prediction;
    Vector3 wantedVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GetInstance().gamePaused && !GameManager.GetInstance().gameOver) {
            //// Glue Timer ////
            if (glueDuration > 0) {
                glueDuration -= Time.deltaTime;
                isGlued = true;
            } else {
                isGlued = false;
            }

            //// Cloak Timer ////
            if (cloakDuration > 0) {
                cloakDuration -= Time.deltaTime;
                GameManager.GetInstance().SetPlayerCloak(true);
            } else {
                GameManager.GetInstance().SetPlayerCloak(false);
            }


            //// Input ////
            // Buttons
            runButton = Input.GetKey("x");

            playerSpeed = defaultWalkSpeed * (runButton ? runSpeedFactor : 1) * (isGlued ? glueSpeedFactor : 1);

            if (Input.GetButtonDown("SmokeBombButton")) {
                UseSmokeBomb();
            }

            if (Input.GetButtonDown("GlueButton")) {
                UseGlue();
            }

            if (Input.GetButtonDown("CloakingDeviceButton")) {
                UseCloakingDevice();
            }

            if (Input.GetButtonDown("StoneButton")) {
                UseStone();
            }
            // Achsen
            wantedVelocity.x = Input.GetAxis("Horizontal") * playerSpeed; 
            wantedVelocity.z = Input.GetAxis("Vertical") * playerSpeed;
        } else {
            wantedVelocity.x = 0;
            wantedVelocity.z = 0;
        }


        prediction = (transform.position - lastPosition) * 20;
        Debug.DrawRay(transform.position, prediction, Color.cyan);
    }

    void LateUpdate() {
        lastPosition = transform.position;
    }

    void FixedUpdate() {
        if (!GameManager.GetInstance().gamePaused && !GameManager.GetInstance().gameOver) {
            wantedVelocity.y = 0;// rigidBody.velocity.y;

            if (wantedVelocity.magnitude > playerSpeed) { // Normalisieren, damit diagonale Laufgeschwindigkeit nicht hoeher ist
                wantedVelocity = wantedVelocity.normalized * playerSpeed;
            }
            Vector3 wantedMovement = Vector3.Lerp(rigidBody.velocity, wantedVelocity, smoothing);
            rigidBody.velocity = wantedMovement;
        }
        GameManager.GetInstance().SetDebugText(GetComponent<Rigidbody>().velocity.magnitude.ToString());
    }

    public Vector3 GetMovementPrediction() {
        return transform.position + prediction;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerSpeed);
    }

    /////// Item Methoden ////////

    void UseSmokeBomb() {
        if (GlobalManager.GetInstance().GetInventoryAsMap()[PlayerItems.smokeBomb] > 0) {

            GuardAI[] guardsList = GameManager.GetInstance().GetGuardAIs();
            foreach (GuardAI guard in guardsList)
            {
                if (Vector3.Distance(transform.position, guard.transform.position) < 2.2f) {
                    guard.SetNextState(GuardAI.State.confused, 1);
                }
            }
            Destroy(Instantiate(SmokeBombPrefab, transform.position, Quaternion.identity), 2);
            GlobalManager.GetInstance().SubtractItem(PlayerItems.smokeBomb);
            GameManager.GetInstance().RefreshItemCount();
        }
    }

    void UseGlue() {
        if (GlobalManager.GetInstance().GetInventoryAsMap()[PlayerItems.glue] > 0) {
            Destroy(Instantiate(GluePrefab, new Vector3(transform.position.x, 0.62359f, transform.position.z), Quaternion.Euler(90,0,Random.Range(0f, 360f))), glueLifeDuration);
            GlobalManager.GetInstance().SubtractItem(PlayerItems.glue);
            GameManager.GetInstance().RefreshItemCount();
        }
    }

    void UseStone () {
        if (GlobalManager.GetInstance().GetInventoryAsMap()[PlayerItems.stone] > 0) {
            GameObject stone = Instantiate(StonePrefab, transform.position+transform.up + (transform.forward*0.5f+transform.up*0.3f), Quaternion.identity);
            stone.GetComponent<StoneEffect>().GiveForce(transform.forward, 500);
            GlobalManager.GetInstance().SubtractItem(PlayerItems.stone);
            GameManager.GetInstance().RefreshItemCount();
        }
    }

    void UseCloakingDevice() {
        if (GlobalManager.GetInstance().GetInventoryAsMap()[PlayerItems.cloakingDevice] > 0) {
            GlobalManager.GetInstance().SubtractItem(PlayerItems.cloakingDevice);
            GameManager.GetInstance().RefreshItemCount();
            cloakDuration = 10f;
        }
    }

    public void SetGlueTimer(float duration) {
        glueDuration = duration;
    }


}

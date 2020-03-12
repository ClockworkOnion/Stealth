using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Ragdoll")]
    public GameObject playerMesh;
    public GameObject RagdollPrefab;
    BoxCollider mainCollider;
    BoxCollider[] childColliders;

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
    public Material cloakMaterial;

    // Glue variablen
    float glueDuration;
    public float glueLifeDuration = 10f;
    public float glueSpeedFactor = 0.5f;
    bool isGlued = false;

    float cloakDuration;

    Animator playerAnimator;
    Rigidbody rigidBody;
    Vector3 lastPosition;
    Vector3 prediction;
    Vector3 wantedVelocity;
    
    // Cloak Effect
    SkinnedMeshRenderer jacketRenderer,  bodyRenderer,  hairRenderer, pantsRenderer, faceRenderer;
    Material jacketMem, hairMem, pantsMem;
    Material[] bodyMems, faceMems;
    Material[] cloakMaterials;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerAnimator = GetComponentInChildren<Animator>();
        mainCollider = GetComponent<BoxCollider>();
        childColliders = GetComponentsInChildren<BoxCollider>();

        // Mesh renderers
        jacketRenderer = GameObject.Find("Jacket").GetComponent<SkinnedMeshRenderer>();
        bodyRenderer = GameObject.Find("Body").GetComponent<SkinnedMeshRenderer>();
        hairRenderer = GameObject.Find("HAir").GetComponent<SkinnedMeshRenderer>();
        pantsRenderer = GameObject.Find("Pants").GetComponent<SkinnedMeshRenderer>();
        faceRenderer = GameObject.Find("Face").GetComponent<SkinnedMeshRenderer>();

        // Alte Materials merken
        jacketMem = jacketRenderer.material;
        hairMem = hairRenderer.material;
        pantsMem = pantsRenderer.material;
        bodyMems = bodyRenderer.materials;
        faceMems = faceRenderer.materials;

        // Arrays vorbereiten
        cloakMaterials = new Material[bodyRenderer.materials.Length];
        for (int i = 0; i < cloakMaterials.Length; i++) {
            cloakMaterials[i] = cloakMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GetInstance().gamePaused && !GameManager.GetInstance().gameOver) { // Spiel ist nicht pausiert
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
                jacketRenderer.material = cloakMaterial;
                hairRenderer.material = cloakMaterial;
                pantsRenderer.material = cloakMaterial;
                bodyRenderer.materials = cloakMaterials;
                faceRenderer.materials = cloakMaterials;
            } else {
                GameManager.GetInstance().SetPlayerCloak(false);
                // Alte Materials resetten
                jacketRenderer.material = jacketMem;
                bodyRenderer.materials = bodyMems;
                hairRenderer.material = hairMem;
                pantsRenderer.material = pantsMem;
                faceRenderer.materials = faceMems;
            }


            //// Input ////
            // Buttons

            // Running
            runButton = Input.GetButton("Run");
            playerSpeed = defaultWalkSpeed * (runButton ? runSpeedFactor : 1) * (isGlued ? glueSpeedFactor : 1);
            playerAnimator.SetBool("isRunning", runButton);

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
        } else { // Spiel ist vorüber oder pausiert
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
        //GameManager.GetInstance().SetDebugText(GetComponent<Rigidbody>().velocity.magnitude.ToString());
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
            cloakDuration = 3f;
        }
    }

    public void SetGlueTimer(float duration) {
        glueDuration = duration;
    }

    public void GetPunched(Transform puncherPosition) {
        // for (int i = 0 ;)
        // Spielstatsus verloren stellen
        GameManager.GetInstance().SetGameLost();
        GlobalManager.GetInstance().LevelLost();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SetFollow(false);

        transform.LookAt(puncherPosition);
        mainCollider.enabled = false;
        playerMesh.SetActive(false);
        GameObject ragdoll = Instantiate(RagdollPrefab,transform.position, Quaternion.identity);
        ragdoll.GetComponent<RagdollControl>().ApplyForce(puncherPosition.position, transform.position);
    }


}

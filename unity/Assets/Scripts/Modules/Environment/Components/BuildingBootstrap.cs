using UnityEngine;

namespace EmergencyAI.Modules.Environment
{
    /// <summary>
    /// Builds a simple 3-room building with corridors, two exits, and a fire
    /// using Unity primitives.
    /// </summary>
    public class BuildingBootstrap : MonoBehaviour
    {
        [SerializeField] private bool buildOnAwake = true;

        public FireHazard Fire { get; private set; }
        public ExitMarker ExitA { get; private set; }
        public ExitMarker ExitB { get; private set; }
        public Transform BuildingRoot { get; private set; }

        private void Awake()
        {
            if (buildOnAwake)
            {
                Build();
            }
        }

        public void Build()
        {
            if (BuildingRoot != null)
            {
                return;
            }

            BuildingRoot = new GameObject("Building").transform;
            BuildingRoot.SetParent(transform);

            CreateFloor();
            CreateRoomsAndWalls();
            ExitA = CreateExit("Exit A", new Vector3(-8f, 0.5f, 0f), blocked: false);
            ExitB = CreateExit("Exit B", new Vector3(8f, 0.5f, 0f), blocked: true);
            Fire = CreateFire(new Vector3(3f, 0.5f, 2.5f), "Room 2", severity: 8);
            EnsureNavMeshSurfaceHint();
        }

        private void CreateFloor()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(BuildingRoot);
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = new Vector3(2.2f, 1f, 1.4f);
            SetColor(floor, new Color(0.35f, 0.35f, 0.38f));
        }

        private void CreateRoomsAndWalls()
        {
            CreateWall("Wall_North", new Vector3(0f, 1.25f, 7f), new Vector3(22f, 2.5f, 0.3f));
            CreateWall("Wall_South", new Vector3(0f, 1.25f, -7f), new Vector3(22f, 2.5f, 0.3f));
            CreateWall("Wall_West", new Vector3(-11f, 1.25f, 0f), new Vector3(0.3f, 2.5f, 14f));
            CreateWall("Wall_East", new Vector3(11f, 1.25f, 0f), new Vector3(0.3f, 2.5f, 14f));
            CreateWall("Divider_1", new Vector3(-3.5f, 1.25f, 2.5f), new Vector3(0.3f, 2.5f, 8f));
            CreateWall("Divider_2", new Vector3(3.5f, 1.25f, -2.5f), new Vector3(0.3f, 2.5f, 8f));
            CreateRoomLabel("Room 1", new Vector3(-7f, 0.1f, 3f));
            CreateRoomLabel("Room 2", new Vector3(0f, 0.1f, 3f));
            CreateRoomLabel("Room 3", new Vector3(7f, 0.1f, -3f));
            CreateRoomLabel("Corridor", new Vector3(0f, 0.1f, -0.5f));
        }

        private void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(BuildingRoot);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
            SetColor(wall, new Color(0.55f, 0.57f, 0.6f));
        }

        private void CreateRoomLabel(string name, Vector3 position)
        {
            var marker = new GameObject(name);
            marker.transform.SetParent(BuildingRoot);
            marker.transform.localPosition = position;
        }

        private ExitMarker CreateExit(string exitName, Vector3 position, bool blocked)
        {
            var exitGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            exitGo.name = exitName;
            exitGo.transform.SetParent(BuildingRoot);
            exitGo.transform.localPosition = position;
            exitGo.transform.localScale = new Vector3(1.2f, 2f, 0.4f);

            var marker = exitGo.AddComponent<ExitMarker>();
            marker.ExitName = exitName;
            marker.IsBlocked = blocked;
            return marker;
        }

        private FireHazard CreateFire(Vector3 position, string location, int severity)
        {
            var fireRoot = new GameObject("FireHazard");
            fireRoot.transform.SetParent(BuildingRoot);
            fireRoot.transform.localPosition = position;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "FireVisual";
            body.transform.SetParent(fireRoot.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1.2f, 0.6f, 1.2f);
            SetColor(body, new Color(1f, 0.35f, 0.05f));

            var flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flame.name = "Flame";
            flame.transform.SetParent(fireRoot.transform);
            flame.transform.localPosition = new Vector3(0f, 1f, 0f);
            flame.transform.localScale = new Vector3(0.9f, 1.2f, 0.9f);
            SetColor(flame, new Color(1f, 0.75f, 0.1f));

            var hazard = fireRoot.AddComponent<FireHazard>();
            hazard.LocationName = location;
            hazard.Severity = severity;
            return hazard;
        }

        private void EnsureNavMeshSurfaceHint()
        {
            var hint = new GameObject("NavMeshBakeHint");
            hint.transform.SetParent(BuildingRoot);
            hint.transform.localPosition = Vector3.zero;
        }

        private static void SetColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
}

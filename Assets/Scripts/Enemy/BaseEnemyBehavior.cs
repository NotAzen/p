using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBehavior : MonoBehaviour
{
    // adjustable settings
    public float health = 100f;
    public float sightRange = 20f;
    public float speed = 5f;

    // line of sight to player
    private GameObject player;
    private bool hasLineOfSight = false;
    public float enemySlowing = 1f;
    private List<Vector2> pathToPlayer = new();
    private Cooldown pathfindingCooldown = new(2f); // cooldown for pathfinding to prevent excessive calculations

    // damage and explosion particles
    [SerializeField] private ParticleSystem damageParticles;
    private ParticleSystem damageParticlesInstance;

    [SerializeField] private ParticleSystem explosionParticles;
    private ParticleSystem explosionParticlesInstance;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    public static void DisableCollision()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
    }

    public static void EnableCollision()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    // enemy explosion whenever it dies
    public void ExplodeEnemy()
    {
        // play explosion particles
        explosionParticlesInstance = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        var explosionMain = explosionParticlesInstance.main;
        explosionMain.startColor = GetComponent<SpriteRenderer>().color;

        Destroy(gameObject);
    }

    // handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // player projectile layer is 8
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Assume the projectile has a ProjectileController script with a damage property
            ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();

            // fail-safe check
            if (projectile != null)
            {
                health -= projectile.damage;
            }

            // play damage particles
            damageParticlesInstance = Instantiate(damageParticles, transform.position, Quaternion.identity);
            // define particle modules
            var main = damageParticlesInstance.main;
            var shape = damageParticlesInstance.shape;

            // set particle color to enemy color
            main.startColor = GetComponent<SpriteRenderer>().color;

            // orient particle shape towards collision point
            Vector2 difference = collision.relativeVelocity;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            shape.rotation = new Vector3(0, 90 - angle, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            ExplodeEnemy();
        }
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    private void FixedUpdate()
    {
        // define vector to player
        Vector3 toPlayerVector = player.transform.position - transform.position;

        if (toPlayerVector.magnitude > sightRange || player.GetComponent<PlayerStatistics>().isSafe)
        {
            // follow path to player if it exists and is not empty
            if (FollowPath())
            {
                return;
            }

            // wander if the path leads to nowhere
            Wander();
            return;
        }

        // check line of sight to player
        RaycastHit2D ray = Physics2D.Raycast(transform.position,
                            toPlayerVector,
                            sightRange,
                            LayerMask.GetMask("Environment", "SafeZone", "Player"));

        // if the raycast hits something, check if it's the player
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.gameObject == player;

            // move towards player if in line of sight
            if (hasLineOfSight)
            {
                FollowPlayer();
            }
            else
            {
                FindPlayer();
            }
            
            // draw debug ray
            Debug.DrawRay(transform.position,
                            toPlayerVector,
                            hasLineOfSight ? Color.green : Color.red);
        }
    }

    private List<Vector2> FindPlayerPath()
    {
        static float Heuristic(Vector2 a, Vector2 b)
        {
            // Manhattan distance heuristic
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        // reconstruct path from a given cameFrom dictionary and current node
        static List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
        {
            List<Vector2> totalPath = new List<Vector2>
            {
                current
            };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current); // prepend to path
            }

            return totalPath;
        }

        static List<Vector2> GetNeighbors(Vector2 node)
        {
            List<Vector2> neighborOffsets = new()
            {
                Vector2.up,
                Vector2.up + Vector2.left,
                Vector2.left,
                Vector2.one * -1,
                Vector2.down,
                Vector2.down + Vector2.right,
                Vector2.right,
                Vector2.one,
            };

            List<Vector2> neighbors = new();

            foreach (Vector2 offset in neighborOffsets)
            {
                Vector2 neighbor = node + offset;
                // check if neighbor is walkable and within bounds of the grid
                RaycastHit2D ray = Physics2D.Raycast(node, offset, 1f, LayerMask.GetMask("Environment", "SafeZone", "Player"));

                if (ray.collider != null)
                {
                    continue; // skip if there's an obstacle in the way
                }

                neighbors.Add(neighbor);
            }

            return neighbors;
        }
        
        static void SortNode(List<Vector2> nodeSet, Dictionary<Vector2, float> nodeScore, Vector2 node)
        {
            // binary search to find the correct position to insert the node based on its score
            int L = 0;
            int R = nodeSet.Count - 1;
            while (L < R)
            {
                int mid = (L + R) / 2;
                if (nodeScore.ContainsKey(nodeSet[mid]) && nodeScore[nodeSet[mid]] < nodeScore[node])
                {
                    L = mid + 1;
                }
                else
                {
                    R = mid;
                }
            }
            nodeSet.Insert(L, node);
        }

        static List<Vector2> AStarAlgorithm(Vector2 start, Vector2 goal)
        {
            // maximum reasonable distance to consider for pathfinding
            float maxDistance = Vector2.Distance(start, goal) + Mathf.Min(25f, Vector2.Distance(start, goal));
            //float maxDistance = 2 * Vector2.Distance(start, goal);

            // nodes that need to be evaluated
            List<Vector2> openSet = new()
            {
                start
            };

            // for a node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start to n currently known
            Dictionary<Vector2, Vector2> cameFrom = new();

            // for a node n, gScore[n] is the cost of the cheapest path from start to n currently known
            Dictionary<Vector2, float> gScore = new()
            {
                [start] = 0f
            };

            // for a node n, fScore[n] = gScore[n] + heuristic(n, goal).
            // fScore[n] represents our current best guess as to how cheap a path could be from start to finish if it goes through n.
            Dictionary<Vector2, float> fScore = new()
            {
                [start] = Heuristic(start, goal)
            };

            while (openSet.Count > 0)
            {
                // get node in openSet with lowest fScore
                Vector2 current = openSet[0];

                // if we reached the goal, reconstruct and return the path
                if ((current - goal).magnitude < 2f)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);
                // for each neighbor of current
                foreach (Vector2 neighbor in GetNeighbors(current))
                {
                    // tentative_gScore is the distance from start to the neighbor through current
                    float tentative_gScore = gScore[current] + Vector2.Distance(current, neighbor);

                    if (tentative_gScore > maxDistance)
                    {
                        continue; // skip if the score is negative (shouldn't happen, but just in case)
                    }

                    if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor])
                    {
                        // this path to neighbor is better than any previous one. Record it!
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
                        if (!openSet.Contains(neighbor))
                        {
                            SortNode(openSet, fScore, neighbor);
                        }
                    }
                }
            }

            return new List<Vector2>(); // no path found
        }

        return AStarAlgorithm(transform.position, player.transform.position);
    }

    private void FindPlayer()
    {
        if (pathfindingCooldown.IsReady())
        {
            pathToPlayer = FindPlayerPath();
            pathfindingCooldown.Trigger();
        }

        // debugging : draw the path
        for (int i = 0; i < pathToPlayer.Count - 1; i++)
        {
            Debug.DrawLine(pathToPlayer[i], pathToPlayer[i + 1], Color.blue, 0.1f);
        }

        FollowPath();
    }

    private bool FollowPath()
    {
        // terminate following path if it's empty
        if (pathToPlayer.Count <= 0)
        {
            return false;
        }

        Debug.Log(pathToPlayer);

        // if pathToPlayer[0] is very close to the enemy, remove it from the path
        if (Vector2.Distance(transform.position, pathToPlayer[0]) < 1f)
        {
            pathToPlayer.RemoveAt(0);
        }

        // terminate following path if it's empty
        if (pathToPlayer.Count <= 0)
        {
            return false;
        }

        Vector2 toPlayerVector = pathToPlayer[0] - (Vector2)transform.position;
        //Debug.Log(toPlayerVector);
        rb.linearVelocity = speed * toPlayerVector.normalized;

        return true;
    }

    private void FollowPlayer()
    {
        Vector2 toPlayerVector = player.transform.position - transform.position;
        rb.linearVelocity = speed * toPlayerVector.normalized;
    }

    private void Wander()
    {
        // drag enemy to a stop when not in line of sight
        rb.linearVelocity *= Mathf.Pow(enemySlowing, Time.deltaTime);

        // move enemy in a random direction every 2 seconds
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.linearVelocity += speed * 0.1f * randomDirection;
    }
}

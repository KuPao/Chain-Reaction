# Chain Reaction

Development Environment: Unity3D, C#

Development Duration: About six months

## Key Technologies:

Used L-system as a grammar rule to describe dynamic scenes, generating near-infinite sequences to explore the vast dynamic scene space.

Since dynamic scenes contain many continuous parameters (such as position, length, and weight), the research breaks down the scene into four parts: event derivation, geometric parameter calculation, physical parameter correction, and simulation & verification.

Integrated Monte Carlo Tree Search (MCTS) to efficiently explore the vast search space, balancing between broad exploration and refining the best sequences and parameters using the Upper Confidence Bound (UCB) formula.

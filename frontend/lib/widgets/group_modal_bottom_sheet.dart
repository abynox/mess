import 'package:flutter/material.dart';
import 'package:mess/provider/group_provider.dart';
import 'package:provider/provider.dart';

class GroupModalBottomSheet extends StatelessWidget {
  const GroupModalBottomSheet({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final groupProvider = context.watch<GroupProvider>();

    return SafeArea(
      top: false,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // List of Groups:
            Flexible(
              child: Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: theme.colorScheme.primaryContainer,
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text("Select your active Group from the list:", style: theme.textTheme.titleMedium),
                    const SizedBox(height: 8),
                    Flexible(
                      child: ListView.builder(
                        shrinkWrap: true,
                        itemCount: groupProvider.groups.length,
                        itemBuilder: (context, index) {
                          return Card(
                            shape: index == groupProvider.activeGroupIndex
                                ? RoundedRectangleBorder(
                                    borderRadius: BorderRadius.circular(12),
                                    side: BorderSide(color: theme.colorScheme.tertiary, width: 2),
                                  )
                                : null,
                            child: ListTile(
                              title: Text("Group: ${groupProvider.groups[index].name}"),
                              onTap: () {
                                print("Tabbed on ${groupProvider.groups[index].name}");
                                groupProvider.setActiveGroupByIndex(index);
                              },
                            ),
                          );
                        },
                      ),
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 8),

            // Buttons
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: theme.colorScheme.primaryContainer,
                borderRadius: BorderRadius.circular(16),
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch, // stretch button over the full width
                children: [
                  FilledButton(
                    onPressed: () => print("Join an existing Group"),
                    child: const Text("Join an existing Group"),
                  ),
                  const SizedBox(height: 8),
                  FilledButton(onPressed: () => print("Create a new Group"), child: const Text("Create a new Group")),
                  const SizedBox(height: 8),
                  FilledButton(onPressed: () => Navigator.pop(context), child: const Text("Back")),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

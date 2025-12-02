import 'dart:collection';

import 'package:flutter/widgets.dart';
import 'package:mess/models/group.dart';
import 'package:mess/services/group_service.dart';

class GroupProvider extends ChangeNotifier {
  List<Group> _groups = [];
  int _activeGroupIndex = -1;

  UnmodifiableListView<Group> get groups => UnmodifiableListView(_groups);

  int get activeGroupIndex => _activeGroupIndex;

  Group? get activeGroup => (_activeGroupIndex == -1 ? null : groups[_activeGroupIndex]);

  void loadGroups() {
    // todo: replace _activeGroupIndex with the right new object

    _activeGroupIndex = -1;
    _groups = GroupService().getGroups();
    if (_groups.isNotEmpty) {
      _activeGroupIndex = 0;
    }
    notifyListeners();
  }

  void setActiveGroupByIndex(int newActiveGroupIndex) {
    _activeGroupIndex = newActiveGroupIndex;
    notifyListeners();
  }
}

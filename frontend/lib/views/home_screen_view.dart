import 'package:flutter/material.dart';

abstract class HomeScreenView extends StatefulWidget {

  const HomeScreenView({super.key});

  FloatingActionButton? buildFloatingActionButton(BuildContext context, VoidCallback rebuildHomeScreen);
}
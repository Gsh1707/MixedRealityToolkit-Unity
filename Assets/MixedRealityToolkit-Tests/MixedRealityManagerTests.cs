﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using NUnit.Framework;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class MixedRealityManagerTests
    {
        public static void CreateMixedRealityManager()
        {
            // Create The MR Manager
            MixedRealityManager.ConfirmInitialized();
        }

        [Test]
        public void Test01_CreateMixedRealityManager()
        {
            CleanupScene();

            // Create The MR Manager
            CreateMixedRealityManager();
            GameObject manager = GameObject.Find(nameof(MixedRealityManager));
            Assert.AreEqual(nameof(MixedRealityManager), manager.name);
        }

        [Test]
        public void Test02_TestNoMixedRealityConfigurationFound()
        {
            LogAssert.Expect(LogType.Error, "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Manager");

            MixedRealityManager.Instance.ActiveProfile = null;

            Assert.IsNull(MixedRealityManager.Instance.ActiveProfile);

            CleanupScene();
        }

        [Test]
        public void Test03_CreateMixedRealityManager()
        {
            InitializeMixedRealityManager();

            // Create Test Configuration
            Assert.IsEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.IsEmpty(MixedRealityManager.Instance.MixedRealityComponents);
        }

        [Test]
        public void Test04_CreateMixedRealityInputManager()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Tests
            Assert.IsNotNull(MixedRealityManager.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.AreEqual(MixedRealityManager.Instance.ActiveProfile.ActiveManagers.Count, 1);
            Assert.IsEmpty(MixedRealityManager.Instance.MixedRealityComponents);
        }

        [Test]
        public void Test05_TestGetMixedRealityInputManager()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            // Retrieve Input System
            var inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();

            // Tests
            Assert.IsNotNull(inputSystem);
        }

        [Test]
        public void Test06_TestMixedRealityInputManagerDoesNotExist()
        {
            InitializeMixedRealityManager();

            //Check for Input System
            var inputSystemExists = MixedRealityManager.Instance.ManagerExists<IMixedRealityInputSystem>();

            // Tests
            Assert.IsFalse(inputSystemExists);
        }


        [Test]
        public void Test07_TestMixedRealityInputManagerExists()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Check for Input System
            var inputSystemExists = MixedRealityManager.Instance.ManagerExists<IMixedRealityInputSystem>();

            // Tests
            Assert.IsTrue(inputSystemExists);
        }

        [Test]
        public void Test08_CreateMixedRealityComponent()
        {
            InitializeMixedRealityManager();

            var component = new TestComponentManager1();

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), component);

            // Tests
            Assert.IsNotNull(MixedRealityManager.Instance.ActiveProfile);
            Assert.IsEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.AreEqual(MixedRealityManager.Instance.MixedRealityComponents.Count, 1);
        }

        [Test]
        public void Test09_TestMixedRealityComponentExists()
        {
            InitializeMixedRealityManager();

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Retrieve Component1
            var component1 = MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager1));

            // Tests
            Assert.IsNotNull(component1);
        }

        [Test]
        public void Test10_TestMixedRealityComponents()
        {
            InitializeMixedRealityManager();

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Retrieve Component1
            var components = MixedRealityManager.Instance.GetManagers(typeof(ITestComponentManager1));

            // Tests
            Assert.AreEqual(components.Count(), 3);
        }

        [Test]
        public void Test11_TestMixedRealityComponent2DoesNotReturn()
        {
            InitializeMixedRealityManager();

            //Add test component
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Validate non-existent component
            var component2 = (TestComponentManager2)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager2), "Test2");

            // Tests
            Assert.IsNull(component2);
        }

        [Test]
        public void Test12_TestMixedRealityComponent2DoesNotExist()
        {
            InitializeMixedRealityManager();

            //Add test component 1
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Validate non-existent component
            var component2 = MixedRealityManager.Instance.ManagerExists<ITestComponentManager2>();

            // Tests
            Assert.IsFalse(component2);
        }

        [Test]
        public void Test13_CreateMixedRealityComponentNameWithInput()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Add test component 1
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

            //Add test component 2
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test2-1" });

            // Tests
            Assert.IsNotNull(MixedRealityManager.Instance.ActiveProfile);
            Assert.IsNotEmpty(MixedRealityManager.Instance.ActiveProfile.ActiveManagers);
            Assert.AreEqual(MixedRealityManager.Instance.ActiveProfile.ActiveManagers.Count, 1);
            Assert.AreEqual(MixedRealityManager.Instance.MixedRealityComponents.Count, 7);
        }

        [Test]
        public void Test14_ValidateComponentNameWithInput()
        {
            InitializeMixedRealityManager();

            // Add Input System
            MixedRealityManager.Instance.AddManager(typeof(IMixedRealityInputSystem), new MixedRealityInputManager());

            //Add test component 1
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1() { Name = "Test14-1" });

            //Add test component 2
            MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager2), new TestComponentManager2 { Name = "Test14-2" });

            //Retrieve Test component 2-2
            TestComponentManager2 component2 = (TestComponentManager2)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager2), "Test14-2");

            // Component 2-2 Tests
            Assert.IsNotNull(component2.InputSystem);
            Assert.AreEqual(component2.Name, "Test14-2");

            //Retrieve Test component 2-1
            TestComponentManager1 component1 = (TestComponentManager1)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager1), "Test14-1");

            // Component 2-1 Tests
            Assert.IsNotNull(component1.InputSystem);
            Assert.AreEqual(component1.Name, "Test14-1");
        }

        [Test]
        public void Test15_GetMixedRealityComponentsCollection()
        {
            InitializeMixedRealityManager();

            //Retrieve Component1
            var components = MixedRealityManager.Instance.GetManagers(typeof(ITestComponentManager2));

            // Tests
            Assert.AreEqual(components.Count(), 2);
        }

        [Test]
        public void Test16_GetAllMixedRealityComponents()
        {
            InitializeMixedRealityManager();

            //Retrieve Component1
            var allComponents = MixedRealityManager.Instance.MixedRealityComponents;

            // Tests
            Assert.AreEqual(allComponents.Count, 9);
        }

        [Test]
        public void Test17_CleanupMixedRealityManager()
        {
            CleanupScene();
        }

        #region Helper Functions

        private static void CleanupScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        private static void InitializeMixedRealityManager()
        {
            CreateMixedRealityManager();

            // Test the Manager
            Assert.IsNotNull(MixedRealityManager.Instance);
            MixedRealityManager.Instance.ActiveProfile = ScriptableObject.CreateInstance<MixedRealityConfigurationProfile>();
            Assert.NotNull(MixedRealityManager.Instance.ActiveProfile);
        }

        #endregion Helper Functions
    }

    #region Test Components

    public interface ITestComponentManager1 : IMixedRealityManager { }

    public interface ITestComponentManager2 : IMixedRealityManager { }

    internal class TestComponentManager1 : BaseManager, ITestComponentManager1
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        public override void Initialize()
        {
            InputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }

    internal class TestComponentManager2 : BaseManager, ITestComponentManager2
    {
        public IMixedRealityInputSystem InputSystem = null;

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        public override void Initialize()
        {
            InputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }

    #endregion Test Components
}
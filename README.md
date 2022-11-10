# Multivariable Grapher, AKA "freematica" (no more $300 per year subscription fees!)

A current project with @Eric Brewster to replace the clunky online interfaces for graphing 3d equations. Supports implicit graphing. The program works by approximating the left and right sides of an equation (substituting x, y, and z values) until they are roughly equivalent, and deciphers the equations using a custom-built equation parser with a linked-list-like structure.

[Unity version 2021.3.3f1]

Here's a snapshot of what it can do (a lot of wacky stuff!):

Hyperboloid:
<img width="1440" alt="1" src="https://user-images.githubusercontent.com/68410154/201023304-7b4cfe93-0992-438a-a474-ac86a452e0c7.png">

Plane:
<img width="1440" alt="2" src="https://user-images.githubusercontent.com/68410154/201023316-90f4f545-fd54-4614-b6bf-d49dd3187ee3.png">

Curves:
<img width="1440" alt="3" src="https://user-images.githubusercontent.com/68410154/201023330-85d0e1e1-2784-4a5f-aa78-ba0f242558dd.png">

Thicc curves:
<img width="1440" alt="4" src="https://user-images.githubusercontent.com/68410154/201023341-4f6c3c98-47b2-4661-afdd-926f41d3626a.png">

Smooth rug:
<img width="1440" alt="5" src="https://user-images.githubusercontent.com/68410154/201023355-9160b107-0dbb-43c2-afb8-72ef4d238a74.png">

The letter "A" (this equation is z = ((1-sign(-x-.9+abs(y * 2)))/3 * (sign(.9-x)+1)/3) * (sign(x+.65)+1)/2 - ((1-sign(-x-.39+abs(y * 2)))/3 * (sign(.9-x)+1)/3) + ((1-sign(-x-.39+abs(y * 2)))/3 * (sign(.6-x)+1)/3) * (sign(x-.35)+1)/2 from https://www.benjoffe.com/code/tools/functions3d/examples):
<img width="1440" alt="6" src="https://user-images.githubusercontent.com/68410154/201023361-ce2b6ef5-da68-4bb0-a665-c5c481c78e66.png">

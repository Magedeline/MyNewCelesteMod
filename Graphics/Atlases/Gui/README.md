# GUI Assets - Title & Branding

## Title Screen Assets

### Main Logo

| File | Dimensions | Description |
|------|------------|-------------|
| `title.png` | 512x256 | Main title logo for title screen |
| `logo.png` | 256x256 | Square logo for menus and loading |

### Menu Backgrounds

| File | Dimensions | Description |
|------|------------|-------------|
| `menu/background.png` | 1920x1080 | Main menu background |
| `menu/gradient.png` | 1920x64 | Gradient overlay for text contrast |
| `creditsGradient.png` | 1920x1080 | Credits screen background gradient |

### Menu Elements

| File | Dimensions | Description |
|------|------------|-------------|
| `menu/button_normal.png` | 200x50 | Normal menu button state |
| `menu/button_hover.png` | 200x50 | Highlighted menu button |
| `menu/button_pressed.png` | 200x50 | Pressed menu button |
| `menu/cursor.png` | 32x32 | Menu cursor/selector |

### Navigation Arrows

| File | Dimensions | Description |
|------|------------|-------------|
| `adjustarrowleft.png` | 16x16 | Left arrow for sliders |
| `adjustarrowright.png` | 16x16 | Right arrow for sliders |
| `tinyarrow.png` | 8x8 | Small navigation arrow |
| `downarrow.png` | 16x16 | Down arrow for dropdowns |
| `towerarrow.png` | 32x32 | Large directional arrow |

### Dots and Indicators

| File | Dimensions | Description |
|------|------------|-------------|
| `dot.png` | 8x8 | Menu dot indicator |
| `dot_outline.png` | 8x8 | Outlined dot variant |
| `dotarrow.png` | 12x12 | Dot with arrow for selection |
| `dotarrow_outline.png` | 12x12 | Outlined dot arrow |
| `dotdotdot.png` | 24x8 | Loading indicator |

### Save Slots

Location: `save/`

| File | Description |
|------|-------------|
| `slot_empty.png` | Empty save slot background |
| `slot_filled.png` | Filled save slot background |
| `slot_selected.png` | Selected save slot highlight |

### Journal Pages

Location: `poempage.png`, `poemside.png`

| File | Description |
|------|-------------|
| `poempage.png` | Journal page background |
| `poemside.png` | Journal spine/binding |

### Postcards

| File | Description |
|------|-------------|
| `postcard.png` | D-Side postcard template |

## DesoloZantas-Specific Assets

Location: `DesoloZantas/`

| File | Description |
|------|-------------|
| `title_custom.png` | Custom mod title |
| `chapter_icons/` | Custom chapter icon sprites |
| `stats_notebook/` | Statistics notebook page assets |
| `dside_postcards/` | D-Side unlock postcard art |

## Area Select Icons

Location: `areas/` and `areaselect/`

Custom chapter icons for the chapter select screen.

## Controls Display

Location: `controls/`

Controller button icons for input prompts.

## Design Guidelines

### Color Palette

- **Primary Gold**: `#FFD700`
- **Accent Cyan**: `#00FFFF`
- **Background Dark**: `#1A1A2E`
- **Text White**: `#FFFFFF`

### Font Sizes

- Title: 64px (Renogare)
- Menu Items: 32px (Renogare)
- Descriptions: 24px (Renogare)

### Animation

- Menu transitions: 0.3s ease-in-out
- Hover effects: 0.15s ease
- Button press: 0.1s linear

## Adding New Assets

1. Create PNG with transparency
2. Place in appropriate subfolder
3. Reference in `SpritesGui.xml` if animated
4. Rebuild mod to include in atlas

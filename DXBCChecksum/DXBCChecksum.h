// DXBCChecksum.h

#pragma once

using namespace System;

namespace DXBCChecksum {

	public ref class DXBCChecksum
	{
      public:
          static array<Int32>^ DXBCChecksum::CalculateDXBCChecksum(array<Byte>^ data);
	};
}

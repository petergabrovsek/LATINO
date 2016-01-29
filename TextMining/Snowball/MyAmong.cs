﻿/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
namespace SF.Snowball
{
	
	public class MyAmong
	{
		public MyAmong(System.String s, int substringI, int result)
		{
			this.sSize = s.Length;
			this.s = s;
			this.substringI = substringI;
			this.result = result;
		}
		
		public int sSize; /* search string */
		public System.String s; /* search string */
		public int substringI; /* index to longest matching substring */
		public int result; /* result of the lookup */
	}
	
}